// D:\PROJETOS\GraphFacil\Library\Services\LotofacilUpdater.cs
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System;
using LotoLibrary.Models;
using Newtonsoft.Json;

﻿

namespace LotoLibrary.Services;

public class LotofacilUpdater
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://servicebus2.caixa.gov.br/portaldeloterias/api/Lotofacil/";

    public LotofacilUpdater()
    {
        _httpClient = new HttpClient();
        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
        _httpClient.DefaultRequestHeaders.Add("Origin", "https://loterias.caixa.gov.br");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");
    }

    public List<Lotofacil> UpdateConcursos(List<Lotofacil> existingConcursos)
    {
        if (existingConcursos == null || existingConcursos.Count == 0)
        {
            throw new ArgumentException("A coleção existente está vazia ou nula.");
        }

        int lastConcursoNumber = existingConcursos.Max(c => c.numero);
        int currentConcurso = lastConcursoNumber + 1;
        bool hasMore = true;

        while (hasMore)
        {
            try
            {
                string url = $"{BaseUrl}{currentConcurso}";
                HttpResponseMessage response = _httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    Lotofacil concurso = JsonConvert.DeserializeObject<Lotofacil>(content);

                    if (concurso != null && concurso.numero == currentConcurso)
                    {
                        existingConcursos.Add(concurso);
                        currentConcurso++;
                    }
                    else
                    {
                        hasMore = false;
                    }
                }
                else
                {
                    hasMore = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter o concurso {currentConcurso}: {ex.Message}");
                hasMore = false;
            }
        }

        return existingConcursos;
    }

}
