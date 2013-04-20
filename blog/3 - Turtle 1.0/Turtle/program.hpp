//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#pragma once

#ifndef WINVER       
#define WINVER 0x0700
#endif

#ifndef _WIN32_WINNT        
#define _WIN32_WINNT 0x0700
#endif

#ifndef UNICODE
#define UNICODE
#endif

#define WIN32_LEAN_AND_MEAN

#include <windows.h>

#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <wchar.h>

#include <d2d1.h>
#include <d2d1helper.h>
#include <dwrite.h>
#include <wincodec.h>

template<class Interface>
inline void SafeRelease(Interface** ppInterfaceToRelease)
{
    if (NULL != *ppInterfaceToRelease)
    {
        (*ppInterfaceToRelease)->Release();
        (*ppInterfaceToRelease) = NULL;
    }
}

template<class Instance>
inline void SafeDelete(Instance* pInstance)
{
	if(nullptr != pInstance)
	{
		delete pInstance;
		pInstance = nullptr;
	}
}

#include "turtle.hpp"

#define WM_ANIMATION_COMPLETE (WM_USER + 0)

struct Animation
{
	Animation::Animation(HWND hwnd, UINT_PTR id = 0);
	~Animation();
	
	void Start(UINT maxFrameCount = 5, UINT elapsed = 1000);	

	const float GetCurrentFrame() const;

	void Update();
	
	const bool IsRunning() const;

	void Stop();

private:
	void Refresh(){ InvalidateRect(Hwnd_, NULL, FALSE); }

private:
	HWND Hwnd_;
	UINT_PTR Id_;
	UINT FrameCount_;
	UINT MaxFrameCount_;
};

struct EventSync
{
	EventSync(bool signaled = false)
	{
		m_IsSignaled = signaled;
		m_Event = CreateEvent(NULL, TRUE, m_IsSignaled, NULL);
	}

	const bool IsSignaled() const { return m_IsSignaled; }

	void Wait(){ WaitForSingleObject(m_Event, INFINITE); };
	void Reset(){ ResetEvent(m_Event); m_IsSignaled = false; }
	void Set(){ SetEvent(m_Event); m_IsSignaled = true; }

private:
	HANDLE m_Event;
	bool m_IsSignaled;
};

//Controller + View
class TurtleApp
{
public:
    TurtleApp();
    ~TurtleApp();

    HRESULT Initialize();
	void Uninitialize();

    void RunMessageLoop();

	void Move(int distance = 1)	
	{ 
		using std::ceil;
		using std::abs;
		
		Synchronize().Wait();
		Synchronize().Reset();

		m_pTurtle->Move(distance);

		int fps_ratio = 30 / m_pTurtle->GetSpeed();

		BeginAnimation
		(
			static_cast<UINT>(ceil(m_pTurtle->GetSize() / 100.0f))  * abs(distance) * fps_ratio, 
			static_cast<UINT>(33.334f)
		);

		Synchronize().Wait();
		//OnEndAnimation do Synchronize().Set()
	}
	
	void Resize(float size)
	{ 
		Synchronize().Wait();
		Synchronize().Reset();

		m_pTurtle->Resize(size);

		Synchronize().Set();
	}
	
	void Rotate(float angleInDegrees) 
	{ 
		Synchronize().Wait();
		Synchronize().Reset();
		
		m_pTurtle->Rotate(angleInDegrees);
		Refresh();

		Synchronize().Set();
	}

	void Speed(int speed)
	{
		Synchronize().Wait();
		Synchronize().Reset();

		m_pTurtle->Speed(speed);

		Synchronize().Set();
	}
	
	void GetDrawableArea(LPRECT lpRect) const	
	{ 
		GetClientRect(m_hwnd, lpRect); 
	}

	void BeginAnimation(UINT frameCount, UINT elapsedTime)
	{ 
		m_pAnimation->Start(frameCount, elapsedTime);
	}

	void Refresh() const
	{
		InvalidateRect(m_hwnd, NULL, FALSE);
	}

	EventSync& Synchronize() 
	{ 
		return m_AnimationSyncEvent;	
	}

private:
    HRESULT CreateDeviceIndependentResources();

    HRESULT CreateDeviceResources();

    void DiscardDeviceResources();

    HRESULT OnRender();

	void DrawTurtlePath(Turtle::TurtleVectorConstIterator& endIterator, float& x0, float& y0);

    void OnTimer(UINT_PTR id);

	void OnEndAnimation(UINT_PTR id);

	static LRESULT CALLBACK WndProc
	(
        HWND hWnd,
        UINT message,
        WPARAM wParam,
        LPARAM lParam
    );

	void RenderTurtleImage(float x, float y);

	HRESULT LoadBitmapFromFile
	(
		ID2D1RenderTarget *pRenderTarget, 
		IWICImagingFactory *pIWICFactory, 
		PCWSTR uri, 
		UINT destinationWidth, 
		UINT destinationHeight, 
		ID2D1Bitmap **ppBitmap
    );

private:
    HWND m_hwnd;
	EventSync m_AnimationSyncEvent;
    ID2D1Factory* m_pDirect2dFactory;
    ID2D1HwndRenderTarget* m_pRenderTarget;
    ID2D1SolidColorBrush* m_pLightSlateGrayBrush;
    ID2D1SolidColorBrush* m_pRedBrush;
	IWICImagingFactory *m_pWICFactory;
	ID2D1Bitmap *m_pBitmap;
	Turtle* m_pTurtle;
	Animation* m_pAnimation;
};