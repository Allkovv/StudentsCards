using System;
using System.Text.Json;
namespace UniversityCore
{
    using UniversityData;

    public class StudentDirectory
    {
        public List<Student> Students { get; set; } = new List<Student>();
        private readonly string _filePath = "students.json";

        public event EventHandler<StudentAdmissionEventArgs> StudentAdmissionProcessed;

        public StudentDirectory()
        {
            LoadFromJson();
        }

        public Student GetStudentByName(string fullName)
        {
            foreach (var student in Students)
            {
                if (student.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase))
                {
                    return student;
                }
            }
            return null;
        }


        public void AddStudent(string fullName, string gender, DateTime enrollmentDate, double averageScore)
        {
            try
            {
                if (Students.Exists(s => s.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Студент с таким именем уже существует.");
                    return;
                }

                Student newStudent = new Student(fullName, gender, enrollmentDate, averageScore);

                if (newStudent.AverageScore >= 4.3)
                {
                    Console.WriteLine($"Студент {newStudent.FullName} успешно зачислен.");
                    Students.Add(newStudent);
                    SaveToJson();
                }
                else
                {
                    newStudent.IsEnrolled = false;
                    Console.WriteLine($"Студент {newStudent.FullName} не прошёл зачисление из-за низкого среднего балла.");
                }


                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении студента: {ex.Message}");
            }
        }

        public void RemoveStudent(string fullName)
        {
            try
            {
                Student studentToRemove = Students.Find(s => s.FullName.Equals(fullName, StringComparison.OrdinalIgnoreCase));

                if (studentToRemove != null)
                {
                    Students.Remove(studentToRemove);
                    SaveToJson();
                    Console.WriteLine($"Студент {fullName} удалён.");
                }
                else
                {
                    Console.WriteLine("Студент не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении студента: {ex.Message}");
            }
        }

        public void SaveToJson()
        {
            string json = JsonSerializer.Serialize(Students, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void LoadFromJson()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                Students = JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
            }
        }

        public void ListStudents()
        {
            if (Students.Count == 0)
            {
                Console.WriteLine("Нет добавленных студентов.");
                return;
            }
            foreach (Student student in Students)
            {
                Console.WriteLine(student);
            }
        }

        public void AddToConsole()
        {
            StudentDirectory directory = new StudentDirectory();
            directory.StudentAdmissionProcessed += (sender, e) => Console.WriteLine($"\n[{e.Student.FullName}] {e.StatusMessage}");
            {
                while (true)
                {
                    Console.WriteLine("\n--- Меню ---");
                    Console.WriteLine("1. Добавить студента");
                    Console.WriteLine("2. Показать всех студентов");
                    Console.WriteLine("3. Удалить студента");
                    Console.WriteLine("4. Узнать на учёбе или нет");
                    Console.WriteLine("5. Статус зачисления");
                    Console.WriteLine("6. Показать студентов по году зачисления");
                    Console.WriteLine("0. Выход");

                    Console.Write("Выберите опцию: ");
                    string option = Console.ReadLine();

                    switch (option)
                    {
                        case "1":
                            Console.Write("Введите полное имя студента: ");
                            string fullName = Console.ReadLine();



                            Console.Write("Введите пол (М/Ж): ");
                            string gender = Console.ReadLine();

                            Console.Write("Введите дату зачисления (yyyy-mm-dd): ");
                            DateTime enrollmentDate;
                            while (!DateTime.TryParse(Console.ReadLine(), out enrollmentDate))
                            {
                                Console.Write("Некорректный формат даты. Повторите ввод (yyyy-mm-dd): ");
                            }

                            Console.Write("Введите средний балл (1,0 - 5,0): ");
                            double averageScore;
                            while (!double.TryParse(Console.ReadLine(), out averageScore) || averageScore < 1.0 || averageScore > 5.0)
                            {
                                Console.Write("Некорректный балл. Повторите ввод (1,0 - 5,0): ");
                            }

                            directory.AddStudent(fullName, gender, enrollmentDate, averageScore);
                            break;

                        case "2":
                            directory.ListStudents();
                            break;

                        case "3":
                            Console.Write("Введите полное имя студента для удаления: ");
                            string removeName = Console.ReadLine();
                            directory.RemoveStudent(removeName);
                            break;
                        case "4":
                            Console.Write("Введите полное имя студента чтобы узнать на учёбе ли он: ");
                            string checkName = Console.ReadLine();
                            var student = directory.GetStudentByName(checkName);

                            if (student != null)
                            {
                                Console.WriteLine($"{student.FullName} {(student.IsEnrolled ? "на учёбе" : "не на учёбе")}");
                            }
                            else
                            {
                                Console.WriteLine("Студент не найден.");
                            }
                            break;
                        case "5":
                            directory.ListEnrolledStatus();
                            break;
                        case "6":
                            Console.Write("Введите год зачисления (yyyy): ");
                            int year;
                            while (!int.TryParse(Console.ReadLine(), out year) || year < 2000 || year > DateTime.Now.Year)
                            {
                                Console.Write("Некорректный год. Повторите ввод (yyyy): ");
                            }
                            directory.ListStudentsByEnrollmentYear(year);
                            break;



                        case "0":
                            return;

                        default:
                            Console.WriteLine("Выберите корректную опцию из меню.");
                            break;
                    }
                }
            }
            
        }

        public void ListEnrolledStatus()
        {
            var studying = Students.Where(s => s.IsEnrolled).ToList();
            var notStudying = Students.Where(s => !s.IsEnrolled).ToList();

            Console.WriteLine("Студенты на учёбе:");
            if (studying.Count == 0)
                Console.WriteLine("  Нет студентов на учёбе.");
            else
                foreach (var s in studying)
                    Console.WriteLine($"  {s.FullName}");

            Console.WriteLine("Студенты не на учёбе:");
            if (notStudying.Count == 0)
                Console.WriteLine("  Нет студентов вне учёбы.");
            else
                foreach (var s in notStudying)
                    Console.WriteLine($"  {s.FullName}");
        }

        public void ListStudentsByEnrollmentYear(int year)
        {
            var studentsInYear = Students.Where(s => s.EnrollmentDate.Year == year).ToList();
            Console.WriteLine($"Студенты, зачисленные в {year} году:");
            if (studentsInYear.Count == 0)
            {
                Console.WriteLine("  Нет студентов, зачисленных в этот год.");
            }
            else
            {
                foreach (var s in studentsInYear)
                {
                    Console.WriteLine($"  {s.FullName}");
                }
            }
        }
    }
}