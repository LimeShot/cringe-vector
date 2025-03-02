using System;

namespace CringeCraft.IO;

class Program {
    static void Main(string[] args) {
        var Tmp = new OutputToSVG(@"C:\myfiles\note.svg");
        Tmp.Export();
        Console.WriteLine("Something happened");
    }
}