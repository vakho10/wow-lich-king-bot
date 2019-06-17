// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"

#include <Windows.h>
#include <metahost.h>
#include <mscoree.h>
#pragma comment(lib, "mscoree.lib")

HRESULT hr;

ICLRMetaHost *pMetaHost = nullptr;
ICLRRuntimeInfo *pRuntimeInfo = nullptr;
ICLRRuntimeHost *pClrRuntimeHost = nullptr;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:

		// build runtime
		hr = CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&pMetaHost));
		hr = pMetaHost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&pRuntimeInfo));
		hr = pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&pClrRuntimeHost));

		// start runtime
		hr = pClrRuntimeHost->Start();

		MessageBox(NULL, L"Hello World!", L"Dll says:", MB_OK);

		// execute managed assembly
		DWORD pReturnValue;
		hr = pClrRuntimeHost->ExecuteInDefaultAppDomain(
			L"ConsoleBot.exe",
			L"ConsoleBot.Program",
			L"Main",
			nullptr, // No arguments
			&pReturnValue);

		break;
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:

		// free resources
		pMetaHost->Release();
		pRuntimeInfo->Release();
		pClrRuntimeHost->Release();
		break;
	}
	return TRUE;
}

