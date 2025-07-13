// D:\PROJETOS\GraphFacil\Library\Services\Auxiliar\SetupResult.cs
﻿using System;

namespace LotoLibrary.Services.Auxiliar
{
    public class SetupResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int FilesCreated { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
}
