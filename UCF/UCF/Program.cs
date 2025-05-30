// See https://aka.ms/new-console-template for more information
using System;


namespace UniversityUI
{
    using System;
    using UniversityCore;

    public class Program
    {
        static void Main(string[] args)
        {

            StudentDirectory directory = new StudentDirectory();
            directory.AddToConsole();
            
        }
    }
}