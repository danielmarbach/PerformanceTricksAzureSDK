// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;

var switcher = new BenchmarkSwitcher(typeof(Program).Assembly);
switcher.Run(args);