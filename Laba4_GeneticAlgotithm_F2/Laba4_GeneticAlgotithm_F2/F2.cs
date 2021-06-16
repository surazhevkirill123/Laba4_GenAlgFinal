using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Laba4_GeneticAlgorithm_F2
{
    class F2
    {
        public static int minNumberInPopulation = -200;
        public static int maxNumberInPopulation = 200;
        public static int sizeOfPopulation = 1000;
        public static int sizeOfIndividual = 5;
        public static int f2Right = 22;
        public static double coeficientOfSelection = 0.5;
        public static int countOfChildrens = 20;
        public static double possibilityOfMutation = 0.1;
        public static Random random = new Random();
        public static int c = 0;
        public static int flagForChildX = 0;
        public static long bestestCoeficientOfSurviving = int.MaxValue;
        public static List<int> bestestIndividual = null;
        public static List<(long, List<int>)> startPopulation = new List<(long, List<int>)>();
        public static List<(long, List<int>)> currentPopulation = startPopulation;
        public static List<(long, List<int>)> pastPopulationAndChildrens = new List<(long, List<int>)>();
        public static List<(long, List<int>)> parents = new List<(long, List<int>)>();
        public static List<(long, List<int>)> childrens = new List<(long, List<int>)>();
        public static List<(long, List<int>)> newPopulation = new List<(long, List<int>)>();
        public static List<double> allPossibilities = new List<double>();
        public static List<double> lineOfPossibilities = new List<double>();


        public static List<int> GetListOfInt(int minNumber, int maxNumber, int count)
        {
            List<int> newListOfInt = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int newNumber = random.Next(minNumber, maxNumber);
                newListOfInt.Add(newNumber);
            }
            return newListOfInt;
        }

        public static string WriteListOfInt(List<int> numbers)
        {
            string ListOfInt = null;
            for (int i = 0; i < numbers.Count; i++)
            {
                ListOfInt += (numbers[i] + " ");
            }
            return ListOfInt;
        }

        public static long F2Left(int u, int w, int x, int y, int z)
        {
            return ((long)Math.Pow(w, 2) * (long)z + (long)u * (long)Math.Pow(z, 2)+ (long)Math.Pow(y, 2) + (long)u * (long)w *(long)Math.Pow(x, 2) * (long)Math.Pow(y, 2) * (long)Math.Pow(z, 2) + (long)w * (long)y * (long)z);
        }

        public static void GetCurrentPopulationResults(List<(long, List<int>)> currentPopulation)
        {
            long bestCoeficientOfSurviving = long.MaxValue;
            List<int> bestIndividual = null;
            for (int i = 0; i < currentPopulation.Count; i++)
            {
                if (currentPopulation[i].Item1 <= bestCoeficientOfSurviving)
                {
                    bestCoeficientOfSurviving = currentPopulation[i].Item1;
                    bestIndividual = currentPopulation[i].Item2;
                }
            }
            bestestCoeficientOfSurviving = bestCoeficientOfSurviving;
            bestestIndividual = bestIndividual;
            Console.WriteLine($"bestCoeficientOfSurviving {c} : {bestestCoeficientOfSurviving} \t\t individual: {WriteListOfInt(bestestIndividual)}");
            c++;
        }


        public static (long, List<int>) CreateChild((long, List<int>) parent1, (long, List<int>) parent2)
        {
            List<int> childNumbers = new List<int>();
            long coeficientOfSurviving = new int();
            for (int j = 0; j < sizeOfIndividual; j++)
            {
                if (random.NextDouble() <= 0.5)
                {
                    childNumbers.Add(parent1.Item2[j]);
                }
                else
                {
                    childNumbers.Add(parent2.Item2[j]);
                }
            }
            /*if(F2Left(childNumbers[0], childNumbers[1], childNumbers[2], childNumbers[3], childNumbers[4]) == 0)
            {
                Console.WriteLine("SW");
            }*/
            coeficientOfSurviving = Math.Abs(f2Right - F2Left(childNumbers[0], childNumbers[1], childNumbers[2], childNumbers[3], childNumbers[4]));
            return ((coeficientOfSurviving, childNumbers));
        }

        public static List<(long, List<int>)> GetNewPopulationFromPossibilities(List<(long, List<int>)> pastPopulationAndChildrens)
        {
            newPopulation.Clear();
            for (int i = 0; i < sizeOfPopulation; i++)
            {
                double denominator = 0;
                for (int j = 0; j < pastPopulationAndChildrens.Count; j++)
                {
                    denominator += ((double)1 / (double)pastPopulationAndChildrens[j].Item1);
                }
                allPossibilities.Clear();
                for (int j = 0; j < pastPopulationAndChildrens.Count; j++)
                {
                    allPossibilities.Add(((double)1 / (double)pastPopulationAndChildrens[j].Item1) / (double)denominator);
                }
                lineOfPossibilities.Clear();
                double mainPossibility = 0;
                for (int j = 0; j < allPossibilities.Count; j++)
                {
                    mainPossibility += allPossibilities[j];
                    lineOfPossibilities.Add(mainPossibility);
                }
                double possibility = random.NextDouble();
                for (int j = 0; j < lineOfPossibilities.Count; j++)
                {
                    if (possibility <= lineOfPossibilities[j])
                    {
                        newPopulation.Add(pastPopulationAndChildrens[j]);
                        pastPopulationAndChildrens.RemoveAt(j);
                        break;
                    }
                }
            }
            return newPopulation;
        }

        static void Main(string[] args)
        {
            //создаём начальную популяцию
            for (int i = 0; i < sizeOfPopulation; i++)
            {
                List<int> numbers = GetListOfInt(minNumberInPopulation, maxNumberInPopulation, sizeOfIndividual);
                long coeficientOfSurviving = Math.Abs(f2Right - F2Left(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4]));
                startPopulation.Add((coeficientOfSurviving, numbers));
            }
            GetCurrentPopulationResults(startPopulation);

            while (bestestCoeficientOfSurviving != 0)
            {
                //выбираем родителей
                currentPopulation.OrderBy(individual => individual.Item1).ToList(); //сортировка по коэфициенту выживаемости по убыванию
                currentPopulation.Reverse();
                parents.Clear();
                for (int i = 0; i < sizeOfPopulation * coeficientOfSelection; i++)
                {
                    parents.Add(currentPopulation[i]);
                }

                //создаём детей
                childrens.Clear();
                for (int i = 0; i < (sizeOfPopulation * coeficientOfSelection) - ((sizeOfPopulation * coeficientOfSelection) % 2); i += 2)
                {
                    for (int j = 0; j < countOfChildrens; j++)
                    {
                        childrens.Add(CreateChild(parents[i], parents[i + 1]));
                    }
                }

                //мутация
                for (int i = 0; i < childrens.Count; i++)
                {
                    for (int j = 0; j < childrens[i].Item2.Count; j++)
                    {
                        if (random.NextDouble() < possibilityOfMutation)
                        {
                            childrens[i].Item2[j] = random.Next(minNumberInPopulation, maxNumberInPopulation);
                        }
                    }
                    long item1 = Math.Abs(f2Right - F2Left(childrens[i].Item2[0], childrens[i].Item2[1], childrens[i].Item2[2], childrens[i].Item2[3], childrens[i].Item2[4]));
                    childrens[i] = ((item1, childrens[i].Item2));
                }

                for (int i = 0; i < childrens.Count; i++)
                {
                    if (childrens[i].Item1 == 0)
                    {
                        bestestCoeficientOfSurviving = childrens[i].Item1;
                        bestestIndividual = childrens[i].Item2;
                        flagForChildX = 1;
                    }
                }

                if (flagForChildX == 1)
                {
                    Console.WriteLine($"Ребёнок поколения {c} : {bestestCoeficientOfSurviving} \t\t individual: {WriteListOfInt(bestestIndividual)}");
                }
                else
                {
                    //замещение
                    pastPopulationAndChildrens.Clear();
                    for (int i = 0; i < currentPopulation.Count; i++)
                    {
                        pastPopulationAndChildrens.Add(currentPopulation[i]);
                    }
                    for (int i = 0; i < childrens.Count; i++)
                    {
                        pastPopulationAndChildrens.Add(childrens[i]);
                    }

                    currentPopulation = pastPopulationAndChildrens;

                    //создаём новую популяцию(размер популяции не меняется)                
                    currentPopulation = GetNewPopulationFromPossibilities(currentPopulation);

                    GetCurrentPopulationResults(currentPopulation);
                }
            }
        }
    }
}
