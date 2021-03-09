using System;
using System.Collections.Generic;
using System.Windows;


namespace Spell_checker
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 3ZJ4B4F9LTG3

    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Коллекция слов из словаря
        string[] dictionary;
        List<string[]> collectionWordsForCheck = new List<string[]>();

        //Параметры Spell_checker
        const int tolerance = 2;// допустимое количество исправлений
        const int maxWordLength = 50;
    
        //Точка входа в логику Spell checker
        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            //Функция отвечает за базовую предобработку входных данных,
            // и инициализацию dictionary,
            // и заполнение collection_words_for_check
            tbOUT.Text = "";

            string[] textInParts = tbIN.Text.Split(new string[] {"==="}, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                dictionary = textInParts[0].Split(new string[] { " ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (IndexOutOfRangeException)
            {
                MessageBox.Show("Input is empty");
            }

            for (int i = 1; i < textInParts.Length; ++i)
            {
                collectionWordsForCheck.Add(textInParts[i].Split(new string[] { " ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            }

            var tree = new BKtree(dictionary.Length, tolerance, maxWordLength);

            for (int i=0; i<dictionary.Length; ++i)
            {
                var tmp = new Node(dictionary[i], maxWordLength);

                tree.Add(tree.root,tmp);
            }

            string textOUT = "";
            string similarWordsStr = "";

            foreach (var collectionWords in collectionWordsForCheck)
            {
                foreach (var word in collectionWords)
                {
                    var similarWords = tree.GetSimilarWords(tree.root, word);
                    tree.canBeBetter = false;

                    similarWordsStr = string.Join(" ", similarWords);
                    if (similarWordsStr == "")
                    {
                        textOUT += "{" + word + "?}";
                    }
                    else if (similarWords.Count > 1)
                    {
                        textOUT += "{" + similarWordsStr + "}";
                    }
                    else
                    {
                        textOUT += similarWordsStr;
                    }
                    textOUT += " ";
                }
                if (collectionWordsForCheck.Count > 1)
                    textOUT += "\n===\n";
            }

            tbOUT.Text = textOUT;
            collectionWordsForCheck.Clear();
           
        }

        

    }

    public class Node
    {
        public string word;
        int maxWordLength;
        public int[] next;

        public Node(string word, int maxWordLength = 50)
        {
            this.word = word;
            this.maxWordLength = maxWordLength;
            next = new int[2 * this.maxWordLength + 2];
            for (int i = 0; i < 2 * this.maxWordLength; ++i)
                next[i] = 0;
        }
    } 

    public class BKtree
    {
        int tolerance;
        int maxWordLength;
        int ptr = 0;
        public Node root = new Node("");
        public Node[] Tree;
        enum ModeGetSimilar { off,on };

        public BKtree(int DictionaryLength, int tolerance = 2, int maxWordLength = 50)
        {
            this.tolerance = tolerance;
            this.maxWordLength = maxWordLength;
            Tree = new Node[DictionaryLength];          
        }

        public int GetEditDistance(string a, string b, Enum mode)
        {
            // Реализация расчёта растояния Левенштейна
            int m = a.Length;
            int n = b.Length;
            int[,] dp = new int[m + 1, n + 1];

            for (int i = 0; i <= m; ++i)
                dp[i, 0] = i;
            for (int j = 0; j <= n; ++j)
                dp[0, j] = j;

            for (int i = 1; i <= m; ++i)
            {
                for (int j = 1; j <= n; ++j)
                {
                    if (a[i - 1] != b[j - 1])
                    {
                        dp[i, j] = Math.Min(1 + dp[i - 1, j],
                                           1 + dp[i, j - 1]);

                    }
                    else
                        dp[i, j] = dp[i - 1, j - 1];

                }
            }

            //Попытка удовлетрить условие о соседствующих удалениях/вставках
            string GetStringDifference(string smallString, string bigString)
            {
                //Возвращает символьную разницу между двумя строками
                string difference = bigString;
                string currentDifference = "";
                bool literaWasIn = false;
                for (int i=0; i < smallString.Length; ++i)
                {
                    for (int j=0; j < difference.Length; ++j)
                    {
                        if (smallString[i] != difference[j])
                            currentDifference += difference[j];
                        else
                        {
                            if (literaWasIn)
                                currentDifference += difference[j];
                            literaWasIn = true;
                        }
                    }

                    difference = currentDifference;
                    currentDifference = "";
                    literaWasIn = false;
                }      
                
                return difference;
            }


            if (mode.Equals(ModeGetSimilar.on) && dp[m, n] == 2)
            {
                string differenceBetweenAandB;

                if (m < n)
                {
                    differenceBetweenAandB = GetStringDifference(a, b); //not a in b
                    if (b.Contains(differenceBetweenAandB))
                    {
                        return maxWordLength*2 - tolerance;
                    }
                }
                else if (n < m)
                {
                    differenceBetweenAandB = GetStringDifference(b, a);//not b in a
                    if (a.Contains(differenceBetweenAandB))
                    {
                        return maxWordLength * 2 - tolerance;
                    }
                }
            }
            return dp[m, n];
        }

        public void Add(Node root,Node curr)
        {
            //Добавление нового узла в дерево
            if (root.word == "")
            {
                this.root = curr;
                Tree[0] = root;
                return;
            }

            int dist = GetEditDistance(curr.word, root.word, ModeGetSimilar.off);

            if (Tree[root.next[dist]].word == "")
            {
                ptr++;
                Tree[ptr] = curr;
                root.next[dist] = ptr;               
            }
            else
            {
                Add(Tree[root.next[dist]], curr);
            }
        }

        //флаг фильтрации
        public bool canBeBetter = false;

        public List<string> GetSimilarWords(Node root, string str) // < change on <=
        {
            List<string> result = new List<string>();
            if (root.word == "")
                return result;

            int dist = GetEditDistance(root.word, str, ModeGetSimilar.off);

            if (dist <= tolerance)
            {
                result.Add(root.word);
                if (dist <= 1) //включаем флаг на фильтрацию
                    canBeBetter = true;
                if (dist == 0) //выходим сразу если есть полное совпадение
                    return result;
            }

            int start = dist - tolerance;

            if (start < 0)
                start = 1;

            // Рекурсивный обход по дочерним узлам с метрикой в пределах
            // [расстояние текушего слова в узле от строчки - допустимая ошибка, 
            //  расстояние текушего слова в узле от строчки + допустимая ошибка]
            while (start <= dist + tolerance)
            {
                List<string> tmp = GetSimilarWords(Tree[root.next[start]], str);

                foreach (var word in tmp)
                {
                    //выходим сразу если есть полное совпадение
                    if (word == str)
                        return new List<string> { word };

                    if (GetEditDistance(word, str, ModeGetSimilar.on) <=2)
                        result.Add(word);

                }
                
                start++;
            }

            //фильтрация
            if (canBeBetter)
            {
                List<string> betterResult = new List<string>();
                foreach(var word in result)
                {
                    if (GetEditDistance(word, str, ModeGetSimilar.on) <= 1)
                        betterResult.Add(word);
                }

                result = betterResult;
            }

            return result;
        }
    }

}
