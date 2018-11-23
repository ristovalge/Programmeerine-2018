using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {

        List<int> list = new List<int>();


        Console.WriteLine("Sisesta nr.");
        string inpute ="";
        int input = 0;
        while (inpute != "0")
        {
             inpute = Console.ReadLine();
            input = int.Parse(inpute);
            list.Add(input);
        }



        Console.WriteLine("Kuvame sisestatu tagurpidi");
        int[] array = list.ToArray();
        //for (int i = 0; i < array.Length; i++) 

        list.Remove(0);
        list.Reverse();
        foreach (int stringValues in list)
        {
            Console.WriteLine(stringValues);      
                                                   
        }

        Console.WriteLine("\nPress any key to exit");
        Console.ReadKey();


    }
      
}