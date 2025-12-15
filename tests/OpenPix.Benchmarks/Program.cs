using BenchmarkDotNet.Running;
using OpenPix.Benchmarks;

// Roda o benchmark
// Run all benchmarks in this assembly
BenchmarkRunner.Run(typeof(Program).Assembly);