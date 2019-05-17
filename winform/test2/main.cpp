//==============================================================
// Forex Strategy Builder
// Copyright (c) Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

#include "stdafx.h"
#include "test.h"
using namespace std;

char *WideToMB(const char *str)
{
	int len = wcslen((const wchar_t*)str) + 1;
	char *out = new char[len];
	size_t convertedChars = 0;
	wcstombs_s(&convertedChars, out, len, (const wchar_t*)str, _TRUNCATE);
	return out;
}

char *CharToWChar(const char *str)
{
	size_t newsize = strlen(str) + 1;
	wchar_t * wcstring = new wchar_t[newsize];
	// Convert char* string to a wchar_t* string.
	size_t convertedChars = 0;
	mbstowcs_s(&convertedChars, wcstring, newsize, str, _TRUNCATE);
	return (char *)wcstring;
}

MTFST_API char *__stdcall testC(char *str)
{
	//return CharToWChar("abcÖÐÎÄ");
	return str;
}