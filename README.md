Цель работы:

Написать программу, которая обрабатывает текстовые файлы, пополняя список встречающихся слов, сохраняя частоту встречаемости слов. По накопленному словарю построить гистограмму встречаемости слов разной длины и проверить закон Ципфа. Для хранения словаря использовать суффиксное дерево.

Описание алгоритма:

Для решения поставленной задачи нам не нужно хранить все суффиксы строки. Достаточно хранить все слова. Поэтому задача построения суффиксного дерева упрощается.
Пусть у нас будет текстовый файл, для которого мы хотим построить частотный словарь. Предварительно разобьем весь текст на слова, исключая знаки препинания и прочие служебные символы, оставляя лишь «-» и «’», если они являются частью слова, и добавим каждому слову уникальный символ «$», обозначающий конец. Теперь в нашем дереве каждый лист гарантированно будет концом слова, а каждое окончание слова – листом.
Будем «идти» посимвольно по каждому слову, запоминаю текущую позицию: узел и символ, с которым сравнивается текущий символ текущего слова. Если символы равны, продвигаемся дальше вглубь дерева, переходя, при необходимости, в потомков текущего узла.
При выявлении несовпадения символов возможны несколько вариантов продолжения:
1.	Если из корня нет пути до первого символа слова, создаем лист из корня, хранящий все слово и устанавливаем его счетчик в 1;
2.	Если из корня есть путь до некоторого символа i текущего слова, но i не равен «$», то есть не является последним символом слова, тогда от текущего узла создаем лист и кладем в него суффикс слова, начинающийся с i+1;
3.	Если i – последний символ слова и текущая позиция - последний символ листа, то просто увеличиваем счетчик текущего узла на 1;
4.	Если i – последний символ слова, но не последний символ, хранящийся в текущем узле, то разбиваем этот узел, оставляя в нем символы до i включительно, а оставшийся «хвост» переносим в новый узел вместе со всеми потомками текущего узла. Тогда новыми потомками текущего узла станут новый узел с перенесенными данными и новый лист (см. п. 2).
Таким образом, мы получим неполное суффиксное дерево, удовлетворяющее нашим нуждам и хранящее частоты всех встречающихся в тексте слов.
Для построения частотного словаря по такому дереву необходимо пройти по всем путям от корня до каждой вершины, записывая символы, хранящиеся в узлах. Будем использовать рекурсию, проходя по каждому ребенку узла.
Так как каждый лист – это конец слова, то счетчик будет больше нуля только в листьях. Это дает нам возможность по значению счетчика определить, запомнили ли мы все слово. Если да, то помещаем его в словарь в качестве ключа, а в значение помещаем значение счетчика. После этого удаляем из слова символы текущего узла и возвращаемся в его родителя, чтобы продолжить обход дерева.
