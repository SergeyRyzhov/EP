#ifndef DIFFDEFRW_H
#define DIFFDEFRW_H

#include <stdarg.h>
#include <stdio.h>

int diffDefReadFile(char* inFile, char* outFile, char* ignorePinExtra,
                    char* ignoreRowName, char* ignoreViaName, char* netSegComp);

#endif
