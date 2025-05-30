using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityData
{
    public class Student
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime GraduationDate { get; set; }
        public bool IsEnrolled { get; set; }
        public double AverageScore { get; set; }

        private static readonly Random _random = new Random();

        public Student(string fullName, string gender, DateTime enrollmentDate, double averageScore)
        {
            FullName = fullName;
            Gender = gender;
            EnrollmentDate = enrollmentDate;
            GraduationDate = enrollmentDate.AddYears(3);
            AverageScore = averageScore;
            IsEnrolled = _random.Next(0, 2) == 1;
        }

        public static bool operator ==(Student a, Student b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (a is null || b is null)
                return false;
            return a.FullName.Equals(b.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(Student a, Student b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Student other && this == other;
        }

        public override int GetHashCode()
        {
            return FullName.ToLower().GetHashCode();
        }

        public override string ToString()
        {
            return $"Имя: {FullName}, Пол: {Gender}, Зачисление: {EnrollmentDate.ToShortDateString()}, " +
                $"Окончание: {GraduationDate.ToShortDateString()} " +
                $"Средний балл при зачислении: {AverageScore:F1}";
        }
    }
        public class StudentAdmissionEventArgs : EventArgs
    {
        public Student Student { get; }
        public string StatusMessage { get; }

        public StudentAdmissionEventArgs(Student student, string statusMessage)
        {
            Student = student;
            StatusMessage = statusMessage;
        }
    }
}