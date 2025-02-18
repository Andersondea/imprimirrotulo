using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;

class Program
{
    static List<string> numeros = new List<string>();
    static int index = 0;
    static Configuracao config;
    static string configFile = "config.json";

    static void Main()
    {
        CarregarConfiguracoes();

        while (true)
        {
            Console.WriteLine("=======================================");
            Console.WriteLine("== IMPRIMIR NA IMPRESSORA DE RÓTULOS ==");
            Console.WriteLine("=======================================");
            Console.WriteLine("\nConfigurações Atuais:");
            Console.WriteLine("Impressora: " + config.NomeImpressora);
            Console.WriteLine("Tamanho da Fonte: " + config.TamanhoFonte);
            Console.WriteLine("Colunas por Rótulo: " + config.ColunasPorRotulo);
            Console.WriteLine("Linhas por Rótulo: " + config.LinhasPorRotulo);
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1) Imprimir arquivo texto");
            Console.WriteLine("2) Imprimir valor digitado");
            Console.WriteLine("3) Configurações");
            Console.WriteLine("9) Limpar tela");
            Console.WriteLine("4) Sair");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1":
                    Console.Write("Digite o nome do arquivo (padrão: pibs.txt): ");
                    string nomeArquivo = Console.ReadLine();
                    if (string.IsNullOrEmpty(nomeArquivo)) nomeArquivo = "pibs.txt";
                    LerArquivo(nomeArquivo);
                    Imprimir();
                    break;
                case "2":
                    Console.Write("Digite o valor a ser impresso: ");
                    string valor = Console.ReadLine();
                    numeros.Clear();
                    numeros.Add(valor);
                    Imprimir();
                    break;
                case "3":
                    Configuracoes();
                    break;
                case "9":
                    Console.Clear();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }

    static void LerArquivo(string caminho)
    {
        try
        {
            numeros = new List<string>(File.ReadAllLines(caminho));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao ler o arquivo: " + ex.Message);
        }
    }

    static void Configuracoes()
    {
        try
        {
            Console.WriteLine("=== CONFIGURAÇÃO ===");
            Console.Write("Digite o nome da impressora (atual: {0}): ", config.NomeImpressora);
            string inputImpressora = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputImpressora)) config.NomeImpressora = inputImpressora;

            Console.Write("Digite o tamanho da fonte (atual: {0}): ", config.TamanhoFonte);
            string inputFonte = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputFonte)) config.TamanhoFonte = float.Parse(inputFonte);

            Console.Write("Digite a quantidade de colunas por rótulo (atual: {0}): ", config.ColunasPorRotulo);
            string inputColunas = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputColunas)) config.ColunasPorRotulo = int.Parse(inputColunas);

            Console.Write("Digite a quantidade de linhas por rótulo (atual: {0}): ", config.LinhasPorRotulo);
            string inputLinhas = Console.ReadLine();
            if (!string.IsNullOrEmpty(inputLinhas)) config.LinhasPorRotulo = int.Parse(inputLinhas);

            SalvarConfiguracoes();
            Console.WriteLine("Configurações atualizadas com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao atualizar configurações: " + ex.Message);
        }
    }

    static void Imprimir()
    {
        index = 0;
        int labelIndex = 1; // Contador de rótulos
        PrintDocument printDoc = new PrintDocument();
        printDoc.PrinterSettings.PrinterName = config.NomeImpressora;

        printDoc.PrintPage += (sender, e) =>
        {
            float margemEsquerda = 10f;
            float espacamentoLinhas = 40f;
            float larguraEtiqueta = 90f;
            float alturaEtiqueta = 60f;
            float deslocamentoConteudo = 30f; // Move o conteúdo para baixo

            for (int i = 0; i < config.LinhasPorRotulo; i++)
            {
                for (int j = 0; j < config.ColunasPorRotulo; j++)
                {
                    if (index < numeros.Count)
                    {
                        float x = margemEsquerda + j * larguraEtiqueta;
                        float y = i * espacamentoLinhas;

                        // Imprime a numeração do rótulo APENAS UMA VEZ por rótulo
                        if (j == 0 && i == 0) // Somente no início do rótulo
                        {
                            e.Graphics.DrawString($"Rótulo {labelIndex}",
                                new Font("Courier New", config.TamanhoFonte),
                                Brushes.Black, x, y);
                           // y += deslocamentoConteudo; // Move o cursor para baixo para o conteúdo
                        }

                        // Imprime os números abaixo do cabeçalho
                        e.Graphics.DrawString(numeros[index++],
                            new Font("Courier New", config.TamanhoFonte, FontStyle.Bold),
                            Brushes.Black, x, y + deslocamentoConteudo);
                    }
                }
            }

            labelIndex++; // Incrementa o número do rótulo para o próximo
            e.HasMorePages = index < numeros.Count;
        };

        try
        {
            printDoc.Print();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao imprimir: " + ex.Message);
        }
    }



    static void CarregarConfiguracoes()
    {
        if (File.Exists(configFile))
        {
            string json = File.ReadAllText(configFile);
            config = JsonSerializer.Deserialize<Configuracao>(json);
        }
        else
        {
            config = new Configuracao();
            SalvarConfiguracoes();
        }
    }

    static void SalvarConfiguracoes()
    {
        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(configFile, json);
    }
}

class Configuracao
{
    public string NomeImpressora { get; set; } = "4BARCODE 4B-2074B"; // "Microsoft Print to PDF";
    public float TamanhoFonte { get; set; } = 8f;
    public int ColunasPorRotulo { get; set; } = 4;
    public int LinhasPorRotulo { get; set; } = 4;
}