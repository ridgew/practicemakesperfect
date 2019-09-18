// dllmain.cpp : 定义 DLL 应用程序的入口点。
#include "stdafx.h"
#include "stdio.h"

#define LogFile "BaseLib.log"
static int g_intDebug = 1;
// a sample exported function
void WriteLog(const char *Msg, int state)
{
	//VS中设置#define _CRT_SECURE_NO_WARNINGS的两种方式

	if (g_intDebug == 1)
	{
		FILE *pf = '\0';
		if (state == 1)
		{
			pf = fopen(LogFile, "w");
		}
		else
		{
			pf = fopen(LogFile, "a");
		}
		fprintf(pf, Msg);
		fprintf(pf, "\n");
		fclose(pf);
	}
}


BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}

void DLL_EXPORT MsgBox(const LPCSTR sometext)
{
	MessageBoxA(0, sometext, "DLL Message", MB_OK | MB_ICONINFORMATION);
}

//https://github.com/jash-git/cmd-call-DLL
double DLL_EXPORT Add(double a, double b)//test_API
{
	WriteLog("\t Add API...", 0);
	FILE *pf = fopen("Ans.txt", "w");
	fprintf(pf, "call Add=%f\n", (a + b));
	fclose(pf);
	return a + b;
}

DLL_EXPORT void CALLBACK Test(HWND hwnd, HINSTANCE hinst, LPSTR lpszCmdLine, int nCmdShow);

void CALLBACK Test(HWND hwnd, HINSTANCE hinst, LPSTR lpszCmdLine, int nCmdShow)
{
	MessageBoxA(hwnd, lpszCmdLine, "RunDll", 48);
}