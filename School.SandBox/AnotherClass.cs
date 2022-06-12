using System;
using School.Audit.Abstractions;

namespace School.SandBox
{
    public class AnotherClass : IAuditable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Lol { get; set; }
    }
}