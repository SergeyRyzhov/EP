#include "stdafx.h"
#include <stdlib.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>
using namespace std;

#ifdef _DEBUG
//#define DEBUG
#endif


vector<string> &split(const string &s, char delim, vector<string> &elems, int removeEmpty = 0) {
  stringstream ss(s);
  string item;
  while (getline(ss, item, delim)) {
    if(removeEmpty && item.length() == 0)
      continue;
    elems.push_back(item);
  }
  return elems;
}

vector<string> split(const string &s, char delim, int removeEmpty = 0) {
  vector<string> elems;
  split(s, delim, elems, removeEmpty);
  return elems;
}

int IsComment(vector<string> parts);

int StartWith(vector<string> parts, string prefix);

class node
{
private:
  int id;
  int sizeX;
  int sizeY;
  int isTerminal;
public:
  node(const char* sid, int x, int y,int t)
  {
    char buffer [80];
    char*id;
    id=strcpy(buffer,sid+1);
    this->id = atoi(id);
    sizeX = x;
    sizeY = y;
    isTerminal = t;
  }
  int Id()
  {
    return id;
  }
  int SizeX()
  {
    return sizeX;
  }
  int SizeY()
  {
    return sizeY;
  }
  int IsTerminal()
  {
    return isTerminal;
  }
  void ToString()
  {
    char buffer [80];
    sprintf (buffer, "%d: (%d x %d)%s", id,sizeX,sizeY,isTerminal > 0?":terminal":"");
#ifdef DEBUG
    printf (buffer);
#endif
  }
};

class nodes
{
public:
  int size;
  node** items;
};

nodes* ReadNodes(char* nodesFile)
{
  string line;
  nodes nodes;
  ifstream myfile (nodesFile);
  int numNodes;
  int numTerminals;
  int i = 0;
  if (myfile.is_open())
  {
    while ( getline (myfile, line) )
    {
      vector<string> parts = split(line,' ',1);
      if(IsComment(parts) == 1)
        continue;

      if(StartWith(parts,"NumNodes"))
      {
        numNodes = atoi(parts[2].c_str());
#ifdef DEBUG
        cout << "Nodes amount: " << numNodes << endl;
#endif
        nodes.size = numNodes;
        nodes.items = new node*[numNodes];
        continue;
      }

      if(StartWith(parts,"NumTerminals"))
      {
        numTerminals = atoi(parts[2].c_str());
#ifdef DEBUG
        cout << "Terminals amount: " << numTerminals << endl;
#endif
        continue;
      }

      auto x = atoi(parts[1].c_str());
      auto y = atoi(parts[2].c_str());
      auto t = parts.size() == 4 ? parts[3].compare(0,4,"term") == 0 ? 1 : 0 : 0;
      nodes.items[i] = new node(parts[0].c_str(),x,y,t);
      nodes.items[i]->ToString();
#ifdef DEBUG
      cout << endl;
#endif
    }
    myfile.close();
  }

  else
    cout << "Unable to open file";
  return &nodes;
}

int StartWith(vector<string> parts, string prefix)
{
  if(parts.size() == 0)
    return 1;
  string firstStr = parts[0];
  auto startWith = firstStr.compare(0,prefix.length(),prefix);
  if(startWith == 0)
    return 1;
  return 0;
}

int IsComment(vector<string> parts)
{
  if(parts.size() == 0)
    return 1;
  string firstStr = parts[0];
  auto iscomment = firstStr.compare(0,4,"UCLA");
  if(iscomment == 0)
    return 1;

  iscomment = firstStr.compare(0,1,"#");
  if(iscomment == 0)
    return 1;

  return 0;
}

class BookshelfModel{
public:
  nodes Nodes;
};

int main (int argc, char* argv[]) {
  /* node* n = new node("p1",1,2,1);
  n->ToString();
  delete n;
  return 0;*/
  /*class Data {
  int    key;
  double value;
  };

  Data x;
  Data *y = new Data[10];

  fstream myFile ("data.bin", ios::in | ios::out | ios::binary);
  myFile.seekg (0);
  myFile.read (reinterpret_cast<char*>(y), sizeof (Data) * 10);

  return 0;*/

  /* if(argc < 2){
  cout << "First argument - file" << endl;
  return 1;
  }*/
  char* nodesFile = "D:\\sryzhov\\prj\\ep\\EP\\benchmarks\\ibmISPD02Bench_Bookshelf\\ibm01\\ibm01.nodes";
  auto ns = ReadNodes(nodesFile);
  return 0;
}

//class Component
//{
//  /// <summary>
//  /// Уникальный номер компонента
//  /// (не совпадает с номером в списке компонентов интегральной схемы)
//  /// </summary>
//public: int id;
//
//        /// <summary>
//        /// Число занимаемых посадочных мест по горизонтали
//        /// </summary>
//public: int sizex;
//
//        /// <summary>
//        /// Число занимаемых посадочных мест по вертикали
//        /// </summary>
//public: int sizey;
//
//private:
//  Component(int id, int sizex, int sizey)
//  {
//    this->id = id;
//    this->sizex = sizex;
//    this->sizey = sizey;
//  }
//
//public: char* ToString()
//        {
//          char* prefix = "Component ";
//          char buffer[20];
//          char *p;
//          p = itoa(id,buffer,10);
//          return strcat(prefix,p);
//        }
//
//        /// <summary>
//        /// Фабрика для создания компонентов интегральной схемы
//        /// </summary>
//        /*public class Pool : Factory<Component>
//        {
//        public void Add(int sizex, int sizey)
//        {
//        Add(new Component(next_id(), sizex, sizey));
//        }
//        }*/
//};
//
//class Field
//{
//  /// <summary>
//  /// Начальный ряд посадочных мест по горизонтали.
//  /// </summary>
//public : int beginx;
//
//         /// <summary>
//         /// Начальный ряд посадочных мест по вертикали.
//         /// </summary>
//public : int beginy;
//
//         /// <summary>
//         /// Число (допустимых для размещения) рядов посадочных мест по горизонтали.
//         /// Нумерация рядов начинается с 0.
//         /// </summary>
//public : int cellsx;
//
//         /// <summary>
//         /// Число (допустимых для размещения) рядов посадочных мест по вертикали.
//         /// Нумерация рядов начинается с 0.
//         /// </summary>
//public : int cellsy;
//
//public: Field(int beginx, int beginy, int cellsx, int cellsy)
//        {
//          this->beginx = beginx;
//          this->beginy = beginy;
//          this->cellsx = cellsx;
//          this->cellsy = cellsy;
//        }
//};
//
//class Net
//{
//  /// <summary>
//  /// Уникальный номер цепи (не совпадает с номером в списке цепей интегральной схемы)
//  /// </summary>
//public : int id;
//
//         /// <summary>
//         /// Элементы цепи
//         /// </summary>
//public : Component* items;
//
//private: Net(int id, Component* items)
//         {
//           this->id = id;
//           this->items = items;
//         }
//
//         /*public override string ToString()
//         {
//         return string.Format("Net id={0} items={1}", id, items.Length);
//         }*/
//
//         /// <summary>
//         /// Фабрика для создания цепей интегральной схемы
//         /// </summary>
//         /*public class Pool : Factory<Net>
//         {
//         public void Add(Component[] items)
//         {
//         Add(new Net(next_id(), items));
//         }
//         }*/
//};