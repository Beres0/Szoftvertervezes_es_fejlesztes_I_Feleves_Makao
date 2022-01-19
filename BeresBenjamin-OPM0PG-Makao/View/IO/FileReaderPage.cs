using Makao.Collections;
using System;
using System.IO;

namespace Makao.View
{
    public abstract class FileReaderPage : Page
    {
        public enum DeleteState
        {
            None, DeleteSuccess, DeleteHasError
        }

        private int partitionIndex;
        protected Controller back;
        protected DeleteState deleteState;
        protected Selector<MakaoFile> fileSelector;
        protected bool hasLoadError;
        protected bool hasReadError;
        protected MakaoFile lastDeletedFile;
        protected int maxItemOnPage;
        protected Selector<int> partitionSelector;
        protected DynamicArray<MakaoFile> CurrentItems { get; private set; }
        protected abstract string DirectoryPath { get; }
        protected DynamicArray<DynamicArray<string>> FilePartitions { get; private set; }

        protected int PartitionIndex
        {
            get => partitionIndex;
            set
            {
                if (!FilePartitions.IsNullOrEmpty())
                {
                    value = Math.Clamp(value, 0, FilePartitions.Count - 1);
                    if (partitionIndex != value)
                    {
                        partitionIndex = value;
                        CurrentItems = PartitionToItems(partitionIndex);
                        fileSelector.ResetController();
                    }
                }
            }
        }

        protected abstract string SearchPattern { get; }

        public FileReaderPage(string name, Application application) : base(name, application)
        {
            maxItemOnPage = 10;
            ReadFiles();
        }

        protected string ItemToString(MakaoFile item)
        {
            return $"Név: {Path.GetFileNameWithoutExtension(item.Path)}\nTípus: {item.GameVariant.Name} Játékosok: {item.NumberOfPlayer}";
        }

        protected DynamicArray<MakaoFile> PartitionToItems(int partitionIndex)
        {
            if (!FilePartitions.IsEmpty)
            {
                DynamicArray<MakaoFile> makaoFiles = new DynamicArray<MakaoFile>();
                for (int i = 0; i < FilePartitions[partitionIndex].Count; i++)
                {
                    MakaoFile file = new MakaoFile(FilePartitions[partitionIndex][i], Application);
                    if (!file.HasError)
                    {
                        makaoFiles.Add(file);
                    }
                }
                return makaoFiles;
            }
            else return null;
        }

        protected void ReadFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(DirectoryPath, SearchPattern);
                DynamicArray<DynamicArray<string>> partitions = new DynamicArray<DynamicArray<string>>();
                int i = 0;
                while (i < files.Length)
                {
                    DynamicArray<string> currentPartition = new DynamicArray<string>();
                    int j = 0;
                    while (i < files.Length && j < maxItemOnPage)
                    {
                        currentPartition.Add(files[i]);
                        j++;
                        i++;
                    }
                    partitions.Add(currentPartition);
                }
                FilePartitions = partitions;
                partitionIndex = 0;
                CurrentItems = PartitionToItems(partitionIndex);
                hasReadError = false;
            }
            catch (IOException)
            {
                hasReadError = true;
                FilePartitions = null;
            }
        }

        protected void ResetErrorFieldsAndDeleteState()
        {
            hasReadError = false;
            hasLoadError = false;
            deleteState = DeleteState.None;
        }

        protected override void SetControllers()
        {
            SetFileSelector();
            SetPartitionSelector();
            controllers.AddController("Frissít").SetCommand(Application.Settings.EnterKey, () => { ReadFiles(); ResetErrorFieldsAndDeleteState(); });
            back = controllers.AddNavigator(() => Application.MainMenu, "Vissza");
        }

        protected void SetFileSelector()
        {
            fileSelector = new Selector<MakaoFile>(() => 0, () => CurrentItems.Count - 1, (i) => CurrentItems[i], null, () => !CurrentItems.IsNullOrEmpty())
                          .SetDecreaseCommand(Application.Settings.UpKey)
                          .SetIncreaseCommand(Application.Settings.DownKey)
                          .SetCommand(Application.Settings.LeftKey, PartitionIndexIncrease, () => PartitionIndex > 0)
                          .SetCommand(Application.Settings.RightKey, PartionIndexDecrease, () => PartitionIndex < FilePartitions.Count - 1)
                          .SetCommand(Application.Settings.DeleteKey, Delete, () => !CurrentItems.IsEmpty);

            fileSelector.WriteMethod = Write;
            controllers.Add(fileSelector);

            void PartitionIndexIncrease()
            {
                PartitionIndex--;
            }
            void PartionIndexDecrease()
            {
                PartitionIndex++;
            }
            void Write()
            {
                if (CurrentItems != null)
                {
                    WriteController(0);
                    for (int i = 1; i < CurrentItems.Count; i++)
                    {
                        ConsoleHelper.WriteSepartorLine('-');
                        WriteController(i);
                    }
                }
            }

            void WriteController(int i)
            {
                if (i == fileSelector.CurrentIndex && controllers.CurrentController == fileSelector)
                {
                    ConsoleHelper.WriteColorizedText(ItemToString(CurrentItems[i]) + "\n", Application.Settings.CurrentColor);
                }
                else
                {
                    Console.WriteLine(ItemToString(CurrentItems[i]));
                }
            }
            void Delete()
            {
                lastDeletedFile = CurrentItems[fileSelector.CurrentIndex];
                CurrentItems.RemoveAt(fileSelector.CurrentIndex);
                FilePartitions[PartitionIndex].RemoveAt(fileSelector.CurrentIndex);

                if (CurrentItems.Count == 0)
                {
                    FilePartitions.RemoveAt(partitionIndex);
                    PartitionIndex = 0;
                }
                try
                {
                    File.Delete(lastDeletedFile.Path);
                    deleteState = DeleteState.DeleteSuccess;
                }
                catch (IOException)
                {
                    deleteState = DeleteState.DeleteHasError;
                }
            }
        }

        protected void SetPartitionSelector()
        {
            partitionSelector = new Selector<int>(() => 0, () => FilePartitions.Count - 1, (i) => PartitionIndex, null, () => FilePartitions.Count > 1);
            partitionSelector.SetCommand(Application.Settings.LeftKey, () => PartitionIndex--, () => PartitionIndex > 0);
            partitionSelector.SetCommand(Application.Settings.RightKey, () => PartitionIndex++, () => PartitionIndex < FilePartitions.Count);
            partitionSelector.WriteMethod = Write;
            controllers.Add(partitionSelector);

            void Write()
            {
                string text = $"{PartitionIndex + 1}.oldal / {FilePartitions.Count}. oldal";
                Console.Write(new string(' ', Console.BufferWidth - text.Length));
                if (partitionSelector == controllers.CurrentController)
                {
                    ConsoleHelper.WriteColorizedText(text + "\n", Application.Settings.CurrentColor);
                }
                else
                {
                    Console.WriteLine(text);
                }
            }
        }

        protected override void WriteContent()
        {
            WriteMessage();
            fileSelector.Write();
            partitionSelector.Write();
            WriteSeparatorLine();
            for (int i = 2; i < controllers.Count; i++)
            {
                controllers[i].Write();
            }
        }

        protected void WriteMessage()
        {
            if (FilePartitions.IsNullOrEmpty())
            {
                Console.WriteLine("Nincsenek fájlok a mappában!");
                WriteSeparatorLine();
            }
            else if (CurrentItems.IsNullOrEmpty())
            {
                Console.WriteLine("Nincsenek fájlok ezen az oldalon!");
                WriteSeparatorLine();
            }
            else if (hasReadError)
            {
                Console.WriteLine("Nem tudtam beolvasni a fájlokat!");
                WriteSeparatorLine();
            }
            else if (deleteState == DeleteState.DeleteSuccess)
            {
                Console.WriteLine($"\"{Path.GetFileName(lastDeletedFile.Path)}\" sikeresen törölve lett!");
                WriteSeparatorLine();
            }
            else if (deleteState == DeleteState.DeleteHasError)
            {
                Console.WriteLine($"\"{Path.GetFileName(lastDeletedFile.Path)}\"-t nem tudtam törölni!");
                WriteSeparatorLine();
            }
            else if (hasLoadError)
            {
                Console.WriteLine($"Fájl betöltése közben hiba történt!");
                WriteSeparatorLine();
            }
        }
    }
}