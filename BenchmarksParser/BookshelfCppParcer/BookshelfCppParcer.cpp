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
namespace bookshelfParser
{
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

  int ReadId(const char* input);

  class Node
  {
  private:
    int id;
    int sizeX;
    int sizeY;
    int isTerminal;
  public:
    Node(const char* sid, int x, int y,int t)
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

  class Nodes
  {
  public:
    int size;
    Node** items;
  };

  class Places
  {
  public:
    int size;
    int* isTerm;
    int* ids;
    int* x;
    int* y;
    int* placed;
  };

  class Net
  {
  public:
    int size;
    int* ids;
  };

  class Nets
  {
  public:
    int size;
    Net** items;
    int numNets;
    int numPins;
  };

  class Bookshelf{
  public:
    Nodes nodes;
    Nets nets;
    Places places;
  };

  int ReadId(const char* input)
  {
    char buffer [80];
    char*id;
    id=strcpy(buffer,input+1);
    return atoi(id);
  }

  Nodes* ReadNodes(const char* nodesFile)
  {
    string line;
    Nodes* nodes = new Nodes();
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
          nodes->size = numNodes;
          nodes->items = new Node*[numNodes];
          continue;
        }
        /* TODO
        if(StartWith(parts,"NumTerminals"))
        {
        numTerminals = atoi(parts[2].c_str());
        #ifdef DEBUG
        cout << "Terminals amount: " << numTerminals << endl;
        #endif
        continue;
        }*/

        auto x = atoi(parts[1].c_str());
        auto y = atoi(parts[2].c_str());
        auto t = parts.size() == 4 ? parts[3].compare(0,4,"term") == 0 ? 1 : 0 : 0;
        nodes->items[i] = new Node(parts[0].c_str(),x,y,t);
        nodes->items[i]->ToString();
#ifdef DEBUG
        cout << endl;
#endif
      }
      myfile.close();
    }

    else
      cout << "Unable to open file";
    return nodes;
  }
  Nets* ReadNets(const char* netsFile)
  {
    Nets* nets = new Nets();
    string line;
    ifstream myfile (netsFile);
    int numNets;
    int numPins;
    int i = 0;
    if (myfile.is_open())
    {
      while ( getline (myfile, line) )
      {
        vector<string> parts = split(line,' ',1);
        if(IsComment(parts) == 1)
          continue;

        if(StartWith(parts,"NumNets"))
        {
          numNets = atoi(parts[2].c_str());
#ifdef DEBUG
          cout << "Nets amount: " << numNets << endl;
#endif
          nets->size = numNets;
          nets->items = new Net*[numNets];
          continue;
        }
        /* TODO
        if(StartWith(parts,"NumPins"))
        {
        numPins = atoi(parts[2].c_str());
        #ifdef DEBUG
        cout << "Pins amount: " << numPins << endl;
        #endif
        continue;
        }*/

        if(StartWith(parts,"NetDegree"))
        {
          int size = atoi(parts[2].c_str());
          Net* n = new Net();
          n->size = size;
          n->ids = new int[size];
#ifdef DEBUG
          cout<<size <<": ";
#endif
          for (int j = 0; j < size; j++)
          {
            getline (myfile, line);
            vector<string> parts = split(line,' ',1);
            n->ids[j] = ReadId(parts[0].c_str());
#ifdef DEBUG
            cout<<n->ids[j] <<" ";
#endif
          }
          nets->items[i++] = n;
#ifdef DEBUG
          cout<<endl;
#endif
          continue;
        }
      }
      myfile.close();
    }

    else
      cout << "Unable to open file";
    return nets;
  }

  Places* ReadPlaces(const char* plFile, int numNodes)
  {
    int dAreaT = INT_MAX;
    int dAreaL = INT_MAX;    
    int dAreaB = INT_MIN;
    int dAreaR = INT_MIN;

    string line;
    Places* places = new Places();
    places->size = numNodes;
    places->isTerm = new int[numNodes];
    places->ids = new int[numNodes];
    places->x = new int[numNodes];
    places->y = new int[numNodes];
    places->placed = new int[numNodes];
    ifstream myfile (plFile);
    int i = 0;
    if (myfile.is_open())
    {
      while ( getline (myfile, line) )
      {
        vector<string> parts = split(line,' ',1);
        if(IsComment(parts) == 1)
          continue;

        int term = parts[0].c_str()[0] == 'p';
        int id = ReadId(parts[0].c_str());
        int x = atoi(parts[1].c_str());
        int y = atoi(parts[2].c_str());
        places->isTerm[i] = term;
        places->ids[i] = id;
        places->x[i] = x;
        places->y[i] = y;

        dAreaT = dAreaT > y ? y : dAreaT;
        dAreaL = dAreaL > x ? x : dAreaT; 
        dAreaB = dAreaB < y ? y : dAreaB;
        dAreaR = dAreaR < x ? x : dAreaR;

        places->placed[i] = x > 0 || y > 0;
#ifdef DEBUG
        if(x > 0 || y > 0)
        {
          cout << (term ? "Terminal e" : "E") << "lement " << id <<" placed at (" <<x<<","<<y<<")."<<  endl;
        }
#endif
      }
      myfile.close();
    }

    else
      cout << "Unable to open file";
    cout << "top:" << dAreaT << endl
      << "left:" << dAreaL<< endl
      << "bottom:" << dAreaB<< endl
      << "right:" << dAreaR<< endl;
    return places;
  }

  Bookshelf* Parse(char* auxFile)
  {
    Bookshelf* model = new Bookshelf();
    string line;
    Nodes nodes;
    ifstream myfile (auxFile);
    int numNodes;
    int numTerminals;
    auto auxStr = (new string(auxFile));
    int lastSlash = auxStr->find_last_of('\\');
    string folder = auxStr->substr(0,lastSlash+1);
    int nodesParsed = 0;
    int netsParsed = 0;
    int placesParsed = 0;
    int sclParsed = 0;
    if (myfile.is_open())
    {
      while ( getline (myfile, line) )
      {
        vector<string> parts = split(line,' ',1);
        if(IsComment(parts) == 1)
          continue;

        if(StartWith(parts,"RowBasedPlacement"))
        {
          for (string file : parts)
          {
            if(file.size() > 5 && nodesParsed == 0)
            {
              if(file.compare(file.size() - 5, 5,"nodes") == 0)
              {
                model->nodes = *ReadNodes((folder + file).c_str());
                cout<<(folder + file)<<endl;
                nodesParsed = 1;
              }
              continue;
            }

            if(file.size() > 4 && netsParsed == 0)
            {
              if(file.compare(file.size() - 4, 4,"nets") == 0)
              {
                model->nets = *ReadNets((folder + file).c_str());
                cout<<(folder + file)<<endl;
                netsParsed = 1;
              }
              continue;
            }

            if(file.size() > 2 && placesParsed == 0 && nodesParsed == 1)
            {
              if(file.compare(file.size() - 2, 2,"pl") == 0)
              {
                model->places = *ReadPlaces((folder + file).c_str(), model->nodes.size);
                cout<<(folder + file)<<endl;
                placesParsed = 1;
              }
              continue;
            }
          }
        }
      }
      myfile.close();
    }
    return model;
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
}

int main (int argc, char* argv[]) {
  char* auxFile = "C:\\projects\\EP\\benchmarks\\ibmISPD02Bench_Bookshelf\\ibm01\\ibm01.aux";
  auto model = bookshelfParser::Parse(auxFile);
  cout<< "Model parsed. " << model->nodes.size << " nodes. " << model->nets.size << " nets." << endl;
  return 0;
}