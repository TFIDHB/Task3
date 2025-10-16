using Microsoft.Extensions.Configuration;
using System;
using Task3.Core;
using Task3.Infrastructure;

namespace Task3
{
    public class Program
    {
        static void ShowMenu()
        {
            Console.WriteLine("\n--- Меню ---");
            Console.WriteLine("1. Показать все задачи");
            Console.WriteLine("2. Показать задачу по ID");
            Console.WriteLine("3. Добавить новую задачу");
            Console.WriteLine("4. Обновить задачу");
            Console.WriteLine("5. Удалить задачу");
            Console.WriteLine("0. Выход");
            Console.Write("Выберите действие: ");
        }

        static string GetTaskStatus(bool isCompleted)
        {
            return isCompleted ? Message.TaskCompleted : Message.TaskIncompleted;
        }

        static string ReadString(string promt, int maxLength, bool allowEmpty)
        {
            
            while (true) {
                Console.Write(promt);
                string input = Console.ReadLine();
                if (!allowEmpty && string.IsNullOrEmpty(input)) {
                    Console.WriteLine(Message.FieldCannotBeEmpty);
                    continue;
                }

                if (input.Length > maxLength) {
                    Console.WriteLine(Message.FieldTooLong, maxLength);
                    continue;
                }

                return input;
            }
        }

        static void ShowAllTasks(IRepository repository)
        {
            var tasks = repository.GetTaskList();
            try
            {
                if (!tasks.Any())
                {
                    Console.WriteLine(Message.NoTasks);
                    return;
                }

                foreach (var task in tasks)
                {
                    Console.WriteLine($"{task.Id}: {task.Title} | {task.Description} | {GetTaskStatus(task.IsCompleted)} | {task.CreatedAt}");
                }
            }

            catch (Exception ex) { 
                Console.WriteLine(Message.TasksLoadError, ex.Message);
            }

        }


        static void ShowTaskById(IRepository repository)
        {
            Console.Write("Введите ID задачи: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int id))
            {
                Console.WriteLine(Message.InvalidID);
                return;
            }

            try
            {
                var task = repository.GetTask(id);
                if (task == null)
                {
                    Console.WriteLine(Message.TaskNotFound);
                    return;
                }

                Console.WriteLine($"ID: {task.Id}");
                Console.WriteLine($"Заголовок: {task.Title}");
                Console.WriteLine($"Описание: {task.Description}");
                Console.WriteLine($"Статус: {GetTaskStatus(task.IsCompleted)}");
                Console.WriteLine($"Создана: {task.CreatedAt}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.TasksLoadError, ex.Message);
            }
        }

        static void AddTask(IRepository repository)
        {
            try
            {
                string title = ReadString("Введите заголовок: ", 255, false);

                Console.Write("Введите описание: ");
                string description = Console.ReadLine();

                var newTask = new TaskClass
                {
                    Title = title,
                    Description = description,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now
                };

                repository.Create(newTask);
                Console.WriteLine(Message.TaskAdded);
            }

            catch (Exception ex)
            {
                Console.WriteLine(Message.Error, ex.Message);
            }

        }

        static void UpdateTask(IRepository repository) {

            Console.Write("Введите ID задачи для обновления: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int updateId))
            {
                Console.WriteLine(Message.InvalidID);
                return;
            }

            try
            {
                var taskToUpdate = repository.GetTask(updateId);
                if (taskToUpdate == null)
                {
                    Console.WriteLine(Message.TaskNotFound);
                    return;
                }

                taskToUpdate.Title = ReadString("Новый заголовок: ", 255, false);

                Console.Write("Новое описание: ");
                taskToUpdate.Description = Console.ReadLine();

                while (true)
                {
                    Console.Write("Завершена? (true/false): ");
                    string? statusInput = Console.ReadLine();

                    if (bool.TryParse(statusInput, out bool isCompleted))
                    {
                        taskToUpdate.IsCompleted = isCompleted;
                        repository.Update(taskToUpdate);
                        Console.WriteLine(Message.TaskUpdated);
                        break;
                    }

                    Console.WriteLine(Message.InvalidStatus);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.Error, ex.Message);
            }
        }
        static void DeleteTask(IRepository repository)
        {
            Console.Write("Введите ID задачи для удаления: ");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int deleteId))
            {
                Console.WriteLine(Message.InvalidID);
                return;
            }

            try
            {
                var task = repository.GetTask(deleteId);
                if (task == null)
                {
                    Console.WriteLine(Message.TaskNotFound);
                    return;
                }

                repository.Delete(deleteId);
                Console.WriteLine(Message.TaskDeleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.Error, ex.Message);
            }
        }
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            IRepository repository = new Repository(configuration);

            while (true)
            {
                ShowMenu();

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowAllTasks(repository);
                        break;

                    case "2":
                        ShowTaskById(repository);
                        break;

                    case "3":
                        AddTask(repository);
                        break;

                    case "4":
                        UpdateTask(repository);
                        break;

                    case "5":
                        DeleteTask(repository);
                        break;

                    case "0":
                        Console.WriteLine("Выход...");
                        return;

                    default:
                        Console.WriteLine(Message.InvalidChoice);
                        break;
                }
            }
        }
    }
}
