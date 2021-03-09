using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Spell_checker.UnitTests
{
    [TestClass]
    public class BKtreeTests
    {
        [TestMethod]
        public void GetSimilarWords_batSattWvbatt_ReturnsSbat()
        {
            //S - это разграничитель слов (separator)
            //W - это разграничитель между словами из словаря и проверяемыми словами
            //Этот метод проверяет если словарь состоит из слов "bat" и "att"
            //А проверяемое слово vbatt, вернуться должен только bat т.к vbatt отличаеться от att двумя подряд вставками

            //Arrange
            string[] dictionary = {"bat", "att"};
            var tree = new BKtree(dictionary.Length);
            for (int i = 0; i < dictionary.Length; ++i)
            {
                Node tmp = new Node(dictionary[i]);

                tree.Add(tree.root, tmp);
            }

            //Act
            var similarWords = tree.GetSimilarWords(tree.root, "vbatt");
            var result = string.Join(" ", similarWords);

            //Assert
            Assert.AreEqual("bat", result);
        }

        [TestMethod]
        public void GetSimilarWords_batSbattWvbatt_ReturnsSbatt()
        {
            //S - это разграничитель слов (separator)
            //W - это разграничитель между словами из словаря и проверяемыми словами
            //Этот метод проверяет если словарь состоит из слов "bat" и "batt"
            //А проверяемое слово vbatt, вернуться должен только batt т.к vbatt отличаеться от bat двумя исправлениями а от batt одним

            //Arrange
            string[] dictionary = { "bat", "batt" };
            var tree = new BKtree(dictionary.Length);
            for (int i = 0; i < dictionary.Length; ++i)
            {
                Node tmp = new Node(dictionary[i]);

                tree.Add(tree.root, tmp);
            }

            //Act
            var similarWords = tree.GetSimilarWords(tree.root, "vbatt");
            var result = string.Join(" ", similarWords);

            //Assert
            Assert.AreEqual("batt", result);
        }

        [TestMethod]
        public void GetSimilarWords_batSvbaWvbatt_ReturnsSbat()
        {
            //S - это разграничитель слов (separator)
            //W - это разграничитель между словами из словаря и проверяемыми словами
            //Этот метод проверяет если словарь состоит из слов "bat" и "vba"
            //А проверяемое слово vbatt, вернуться должен только bat т.к vba отличаеться от bat двумя вставками подряд

            //Arrange
            string[] dictionary = { "bat", "vba" };
            var tree = new BKtree(dictionary.Length);
            for (int i = 0; i < dictionary.Length; ++i)
            {
                Node tmp = new Node(dictionary[i]);

                tree.Add(tree.root, tmp);
            }

            //Act
            var similarWords = tree.GetSimilarWords(tree.root, "vbatt");
            var result = string.Join(" ", similarWords);

            //Assert
            Assert.AreEqual("bat", result);
        }

        [TestMethod]
        public void GetSimilarWords_a50Sab48aWb48_ReturnsSab48a()
        {
            //S - это разграничитель слов (separator)
            //W - это разграничитель между словами из словаря и проверяемыми словами
            //Этот метод проверяет экстремальный случай когда в словаре лежит слово len("aaaaa...") = 50
            //И len("ab...ba") = 50
            //А проверяемое слово len("b...b") = 48, должно получиться "ab....ba"

            //Arrange
            string[] dictionary = { "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa ",
                "abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbba" };
            var tree = new BKtree(dictionary.Length);
            for (int i = 0; i < dictionary.Length; ++i)
            {
                Node tmp = new Node(dictionary[i]);

                tree.Add(tree.root, tmp);
            }

            //Act
            var similarWords = tree.GetSimilarWords(tree.root, "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
            var result = string.Join(" ", similarWords);

            //Assert
            Assert.AreEqual("abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbba", result);
        }

        [TestMethod]
        public void GetSimilarWords_a50Sb48Wab48a_ReturnsSb48()
        {
            //S - это разграничитель слов (separator)
            //W - это разграничитель между словами из словаря и проверяемыми словами
            //Этот метод проверяет экстремальный случай когда в словаре лежит слово len("aaaaa...") = 50
            //И len("bbbb...") = 48
            //А проверяемое слово len("ab...ba") = 50, должно получиться "bbbb..."

            //Arrange
            string[] dictionary = { "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb" };
            var tree = new BKtree(dictionary.Length);
            for (int i = 0; i < dictionary.Length; ++i)
            {
                Node tmp = new Node(dictionary[i]);

                tree.Add(tree.root, tmp);
            }

            //Act
            var similarWords = tree.GetSimilarWords(tree.root, "abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbba");
            var result = string.Join(" ", similarWords);

            //Assert
            Assert.AreEqual("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", result);
        }
    }
}
