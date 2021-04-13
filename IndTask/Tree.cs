using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace IndTask
{
    class Tree
    {
        internal Node root;
        internal List<List<char>> input;
        private Position currentPosition;

        public Tree(string path)
        {
            if (File.Exists(path))
            {
                string str;
                using (StreamReader sr = new StreamReader(path))
                {
                    str = sr.ReadToEnd();
                }

                str = str.Replace('.', ' ').Replace(',', ' ').Replace('?', ' ').Replace('!', ' ').Replace(" - ", " ").Replace(':', ' ').Replace(';', ' ').Replace('(', ' ').Replace(')', ' ').Replace('\"', ' ').Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ').ToLower();
                while (str.IndexOf("  ") != -1)
                {
                    str = str.Replace("  ", " ");
                }

                var array = str.Split().ToList();
                if (array[array.Count - 1] == " " || array[array.Count - 1] == "")
                {
                    array.RemoveRange(array.Count - 1, 1);
                }


                if (array[0] == "" || array[0] == " ")
                {
                    array.RemoveRange(0, 1);

                }

                this.input = new List<List<char>>();

                for (int i = 0; i < array.Count; i++)
                {
                    var word = new List<char>();
                    word = (array[i]).ToCharArray().ToList();
                    word.Add('$');
                    this.input.Add(word);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public void Build()
        {
            root = new Node();
            currentPosition = new Position(root, 0);

            for (int i = 0; i < input.Count; i++)
            {
                StartPhase(i);
            }
        }

        private void StartPhase(int i)

        {
            if (!currentPosition.node.children.Any())
            {
                CreateLeafFromRoot(i);
                return;
            }
            bool flag = false;
            bool checker = false;
            //иду по детям
            foreach (var item in currentPosition.node.children)
            {
                if (item.symbols[0] == input[i][0])//если есть слово       
                {
                    checker = false;
                    flag = false;
                    int index = -1;
                    currentPosition.node = item;
                    currentPosition.pos = index;
                    Position temp = new Position(root, -1);

                    for (int j = 0; j < input[i].Count; j++)
                    {
                        if (currentPosition.node.symbols.Count < currentPosition.pos + 1 && currentPosition.pos < input[i].Count)
                        {
                            if (currentPosition.node.children.Any())
                            {
                                bool localFlag = false;
                                Node localNode = new Node();
                                localNode = currentPosition.node;

                                temp.node = currentPosition.node;
                                temp.pos = currentPosition.pos;

                                foreach (var child in localNode.children)
                                {
                                    if (!localFlag)
                                    {
                                        currentPosition.node = child;
                                        currentPosition.pos = 0;
                                        if (NextChar(i, j, input[i]))
                                        {
                                            localFlag = true;
                                            break;
                                        }
                                        else
                                        {
                                            currentPosition.node = temp.node;
                                            currentPosition.pos = temp.pos;
                                        }
                                    }
                                }
                                if (!localFlag)
                                {
                                    checker = true;
                                }

                            }
                        }

                        if (checker)
                        {
                            //создаю лист стекущего символа
                            Node newLeaf = new Node();
                            newLeaf.symbols = input[i].GetRange(j, input[i].Count - j);
                            newLeaf.counter = 1;
                            newLeaf.startIndex = j;
                            newLeaf.endIndex = input[i].Count - 1;

                            currentPosition.node.children.Add(newLeaf);
                            break;

                        }
                        else
                        {
                            if (NextChar(i, j, input[i]))
                            {
                                currentPosition.pos++;
                                index = j;
                            }
                            else
                            {
                                index = j;
                                break;
                            }
                        }
                    }
                    //в зависимости от j решаем, что делать дальше
                    if (!checker)
                    {
                        Decide(i, index);
                    }
                    break;
                }
                else
                {
                    flag = true;
                }

                currentPosition.node = root;
                currentPosition.pos = 0;
            }

            if (flag)
            {
                CreateLeafFromRoot(i);
            }
            currentPosition.node = root;
            currentPosition.pos = 0;

        }

        private void CreateLeafFromRoot(int i)
        {
            Node node = new Node();
            foreach (var ch in input[i])
            {
                node.symbols.Add(ch);
            }
            node.startIndex = 0;
            node.endIndex = node.symbols.Count - 1;
            node.counter = 1;
            currentPosition.node.children.Add(node);
        }

        private void CreateLeaf(int i, int j)
        {
            Node node = new Node();
            for (int z = j; z < input[i].Count; z++)
            {
                node.symbols.Add(input[i][z]);
            }
            node.startIndex = j;
            node.endIndex = input[i].Count - 1;
            node.counter = 1;
            currentPosition.node.children.Add(node);
        }

        private void Decide(int i, int index)
        {
            //j==0 => нет первого символа => создаем новый лист от корня
            //j==input[i].Length-1 => слово уже хранится => увеличиваем его счетчик
            // 0 < j < input[i].Length => нужно разбивать суффикс
            if (index == 0)
            {
                CreateLeafFromRoot(i);
            }
            else if (index == input[i].Count - 1 && index == currentPosition.node.endIndex && currentPosition.node.counter != 0)
            {
                currentPosition.node.counter++;
            }
            else
            {
                //разбиваем с равного символа, сохраняем остаток                
                if (currentPosition.node.symbols.Count - 1 != 0)
                {
                    Node nodeContinue = new Node();

                    //меняем счетчики
                    nodeContinue.counter = currentPosition.node.counter;
                    currentPosition.node.counter = 0;

                    //меняем индексы
                    nodeContinue.startIndex = index;

                    nodeContinue.endIndex = currentPosition.node.endIndex;
                    currentPosition.node.endIndex = index - 1;

                    //меняем детей
                    nodeContinue.children = currentPosition.node.children;
                    currentPosition.node.children = new List<Node>();

                    //мменяем содержимое
                    nodeContinue.symbols = currentPosition.node.symbols.GetRange(currentPosition.pos, currentPosition.node.symbols.Count - currentPosition.pos);
                    currentPosition.node.symbols.RemoveRange(currentPosition.pos, currentPosition.node.symbols.Count - currentPosition.pos);

                    //добавляем старый хвост в детей
                    currentPosition.node.children.Add(nodeContinue);

                    //создаем лист с остатком текущего слова
                    CreateLeaf(i, index);
                }
                else//если текущее слово целиком содержит то, что есть в активном листе
                {
                    //создаем лист с остатком текущего слова
                    CreateLeaf(i, index);
                }
            }
        }

        /// сравнивает текущий символ слова и соответствующий символ на этой ветке
        private bool NextChar(int i, int j, List<char> word)
        {
            if (currentPosition.pos < 0)
            {
                currentPosition.pos = 0;
            }
            if (currentPosition.node.symbols[currentPosition.pos] == input[i][j])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Traversal(Node node, List<char> word, Dictionary<string, double> storage)
        {
            if (node == null)
            {
                return;
            }

            foreach (var ch in node.symbols)
            {
                word.Add(ch);
            }

            //если лист - вывожу
            if (node.counter != 0)
            {
                word.Remove(word[word.Count - 1]);
                string key = "";
                foreach (var ch in word)
                {
                    key += ch;
                }
                storage.Add(key, (double)node.counter/input.Count);
            }
            else//иначе иду в детей
            {
                foreach (var child in node.children)
                {
                    Traversal(child, word, storage);

                    if (child.symbols[child.symbols.Count - 1] == '$')
                    {
                        word.RemoveRange(child.startIndex, child.symbols.Count - 1);
                    }
                    else
                    {
                        word.RemoveRange(child.startIndex, child.symbols.Count);
                    }
                }

            }
        }
    }
}
