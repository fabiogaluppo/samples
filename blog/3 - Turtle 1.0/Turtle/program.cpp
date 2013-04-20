//Code provided by Fabio Galuppo 
//April 2012
//A homage to: 
//Logo (http://el.media.mit.edu/logo-foundation/logo/programming.html) and 
//Small Basic(http://msdn.microsoft.com/en-us/beginner/gg605166.aspx)

#include "program.hpp"

#include "mathhelper.hpp"

Animation::Animation(HWND hwnd, UINT_PTR id) :
	Hwnd_(hwnd),	
	Id_(id),
	FrameCount_(0),
	MaxFrameCount_(0)
{
}

Animation::~Animation()
{
	Stop();
}
	
void Animation::Start(UINT maxFrameCount, UINT elapsed)
{
	MaxFrameCount_ = maxFrameCount;
	FrameCount_ = 1;
	Refresh();

	SetTimer(Hwnd_, Id_, elapsed, reinterpret_cast<TIMERPROC>(NULL));
}

const float Animation::GetCurrentFrame() const { return static_cast<float>(FrameCount_) / MaxFrameCount_; }

void Animation::Update()
{
	if(FrameCount_++ > MaxFrameCount_)
	{
		Stop();
	}
	else
	{
		Refresh();
	}
}

const bool Animation::IsRunning() const  
{ 
	return FrameCount_ > 0; 
}

void Animation::Stop()
{
	MaxFrameCount_ = FrameCount_ = 0;
	KillTimer(Hwnd_, Id_);

	PostMessage(Hwnd_, WM_ANIMATION_COMPLETE, Id_, 0);
}

TurtleApp::TurtleApp() :
    m_hwnd(nullptr),	
	m_pDirect2dFactory(nullptr),
    m_pRenderTarget(nullptr),
    m_pLightSlateGrayBrush(nullptr),
	m_pRedBrush(nullptr),   
	m_pWICFactory(nullptr),   
    m_pBitmap(nullptr),
	m_pTurtle(nullptr),
	m_pAnimation(nullptr)
{
}

TurtleApp::~TurtleApp()
{
	SafeRelease(&m_pDirect2dFactory);
	SafeRelease(&m_pRenderTarget);
    SafeRelease(&m_pLightSlateGrayBrush);
	SafeRelease(&m_pRedBrush);
    SafeRelease(&m_pWICFactory);    
	SafeRelease(&m_pBitmap);

	SafeDelete(m_pTurtle);
	SafeDelete(m_pAnimation);
}

HRESULT TurtleApp::Initialize()
{
    HRESULT hr;

    hr = CreateDeviceIndependentResources();

    if (SUCCEEDED(hr))
    {
        const wchar_t* className = L"D2DTurtleApp";
		
		WNDCLASSEX wcex = { sizeof(WNDCLASSEX) };
        wcex.style         = CS_HREDRAW | CS_VREDRAW;
        wcex.lpfnWndProc   = TurtleApp::WndProc;
        wcex.cbClsExtra    = 0;
        wcex.cbWndExtra    = sizeof(LONG_PTR);
        wcex.hInstance     = NULL;
        wcex.hbrBackground = NULL;
        wcex.lpszMenuName  = NULL;
        wcex.hCursor       = LoadCursor(NULL, IDI_APPLICATION);
        wcex.lpszClassName = className;

        RegisterClassEx(&wcex);

        FLOAT dpiX, dpiY;

        m_pDirect2dFactory->GetDesktopDpi(&dpiX, &dpiY);

        m_hwnd = CreateWindowEx
		(
			WS_EX_TOPMOST,
			className,
            L"Turtle Graphics by Fabio Galuppo (http://fabiogaluppo.wordpress.com)",
            WS_OVERLAPPED | WS_CAPTION,
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            static_cast<UINT>(ceil(640.f * dpiX / 96.f)),
            static_cast<UINT>(ceil(480.f * dpiY / 96.f)),
            NULL,
            NULL,
            NULL,
            this
        );
        
		hr = m_hwnd ? S_OK : E_FAIL;

		if (SUCCEEDED(hr))
        {
			RECT rc;
			GetDrawableArea(&rc);
			
			m_pTurtle = new Turtle((rc.bottom - rc.top) / 2.0f, (rc.right - rc.left) / 2.0f);
			m_pAnimation = new Animation(m_hwnd);
		
			ShowWindow(m_hwnd, SW_SHOWNORMAL);
            UpdateWindow(m_hwnd);
        }
    }

    return hr;
}

void TurtleApp::Uninitialize()
{
	ShowWindow(m_hwnd, SW_HIDE);
	UpdateWindow(m_hwnd);

	PostMessage(m_hwnd, WM_DESTROY, 0, 0);
}

HRESULT TurtleApp::CreateDeviceIndependentResources()
{
    HRESULT hr = S_OK;

    hr = D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, &m_pDirect2dFactory);

	if (SUCCEEDED(hr))
    {
        hr = CoCreateInstance(CLSID_WICImagingFactory, NULL, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&m_pWICFactory));
    }

    return hr;
}

HRESULT TurtleApp::CreateDeviceResources()
{
    HRESULT hr = S_OK;

    if (!m_pRenderTarget)
    {
        RECT rc;
        GetClientRect(m_hwnd, &rc);

        D2D1_SIZE_U size = D2D1::SizeU(rc.right - rc.left, rc.bottom - rc.top);

		hr = m_pDirect2dFactory->CreateHwndRenderTarget
			(
				D2D1::RenderTargetProperties(),
				D2D1::HwndRenderTargetProperties(m_hwnd, size),
				&m_pRenderTarget
            );


		if(SUCCEEDED(hr))
		{
			hr = LoadBitmapFromFile
				(
					m_pRenderTarget,
					m_pWICFactory,
					L"turtle.png",
					0,
					0,
					&m_pBitmap
                );
		}

        if (SUCCEEDED(hr))
        {
            hr = m_pRenderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::LightSlateGray), &m_pLightSlateGrayBrush );
        }

		if (SUCCEEDED(hr))
        {
            hr = m_pRenderTarget->CreateSolidColorBrush( D2D1::ColorF(D2D1::ColorF::Red), &m_pRedBrush );
        }
    }

    return hr;
}

void TurtleApp::DiscardDeviceResources()
{
    SafeRelease(&m_pRenderTarget);
    SafeRelease(&m_pLightSlateGrayBrush);
	SafeRelease(&m_pRedBrush);
	SafeRelease(&m_pBitmap);
}

void TurtleApp::RunMessageLoop()
{
    MSG msg;

    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
}

void TurtleApp::OnTimer(UINT_PTR id)
{
	if(nullptr != m_pAnimation)
		m_pAnimation->Update();
}

void TurtleApp::OnEndAnimation(UINT_PTR id)
{
	Synchronize().Set();
}

void TurtleApp::DrawTurtlePath(Turtle::TurtleVectorConstIterator& endIterator, float& x0, float& y0)
{
	auto it = m_pTurtle->Begin();
	
	x0 = it->x;
	y0 = it->y;
			
	for(++it; it != endIterator; ++it)
	{
		auto x1 = it->x;
		auto y1 = it->y;

		m_pRenderTarget->DrawLine
		(
			D2D1::Point2F(x0, y0),
			D2D1::Point2F(x1, y1),
			m_pRedBrush, 2.0f
		);

		x0 = x1;
		y0 = y1;
	}
}

HRESULT TurtleApp::OnRender()
{
    HRESULT hr = S_OK;

    hr = CreateDeviceResources();

    if (SUCCEEDED(hr))
    {
        m_pRenderTarget->BeginDraw();

        m_pRenderTarget->SetTransform(D2D1::Matrix3x2F::Identity());

        m_pRenderTarget->Clear(D2D1::ColorF(D2D1::ColorF::White));

        D2D1_SIZE_F rtSize = m_pRenderTarget->GetSize();

        int width = static_cast<int>(rtSize.width);
        int height = static_cast<int>(rtSize.height);

        for (int x = 0; x < width; x += 10)
        {
            m_pRenderTarget->DrawLine
			(
                D2D1::Point2F(static_cast<FLOAT>(x), 0.0f),
                D2D1::Point2F(static_cast<FLOAT>(x), rtSize.height),
                m_pLightSlateGrayBrush,
                0.5f
            );
        }

        for (int y = 0; y < height; y += 10)
        {
            m_pRenderTarget->DrawLine
			(
                D2D1::Point2F(0.0f, static_cast<FLOAT>(y)),
                D2D1::Point2F(rtSize.width, static_cast<FLOAT>(y)),
                m_pLightSlateGrayBrush,
                0.5f
            );
        }
		
		if( nullptr != m_pTurtle && nullptr != m_pAnimation )
		{
			float x0, y0, dx, dy;
			
			if(m_pAnimation->IsRunning())
			{
				DrawTurtlePath(m_pTurtle->InitEnd(), x0, y0);
				
				//update animation
				auto it2 = m_pTurtle->InitEnd();
				auto f = m_pAnimation->GetCurrentFrame();
				auto x1 = it2->x;
				auto y1 = it2->y;
				dx = lerp(x0, x1, f);
				dy = lerp(y0, y1, f);

				m_pRenderTarget->DrawLine
				(
						D2D1::Point2F(x0, y0),
						D2D1::Point2F(dx, dy),
						m_pRedBrush, 2.0f
				);
			}
			else
			{
				float x0, y0;
				DrawTurtlePath(m_pTurtle->End(), x0, y0);

				auto it = m_pTurtle->InitEnd();
				dx = it->x;
				dy = it->y;
			}

			m_pRenderTarget->SetTransform(D2D1::Matrix3x2F::Rotation(m_pTurtle->GetAngleInDegrees(), D2D1::Point2F(dx, dy)));
			RenderTurtleImage(dx, dy);
		}
		
        hr = m_pRenderTarget->EndDraw();
    }

    if (hr == D2DERR_RECREATE_TARGET)
    {
        hr = S_OK;
        DiscardDeviceResources();
    }

    return hr;
}

LRESULT CALLBACK TurtleApp::WndProc(HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    LRESULT result = 0;

    if (message == WM_CREATE)
    {
        LPCREATESTRUCT pcs = reinterpret_cast<LPCREATESTRUCT>(lParam);
        TurtleApp *pApp = reinterpret_cast<TurtleApp*>(pcs->lpCreateParams);
        ::SetWindowLongPtrW(hwnd, GWLP_USERDATA, PtrToUlong(pApp));
        result = 1;
    }
    else
    {
        TurtleApp* pApp = reinterpret_cast<TurtleApp*>(static_cast<LONG_PTR>(::GetWindowLongPtrW(hwnd, GWLP_USERDATA)));

        bool wasHandled = false;

        if (pApp)
        {
            switch (message)
            {
			case WM_TIMER:
				pApp->OnTimer(static_cast<UINT_PTR>(wParam));
				wasHandled = true;
				break;
			case WM_ANIMATION_COMPLETE:
				pApp->OnEndAnimation(static_cast<UINT_PTR>(wParam));
				wasHandled = true;
				break;
            case WM_DISPLAYCHANGE:
                InvalidateRect(hwnd, NULL, FALSE);
                wasHandled = true;
                break;
            case WM_PAINT:
				pApp->OnRender();
                ValidateRect(hwnd, NULL);
                wasHandled = true;
                break;
            case WM_DESTROY:
				PostQuitMessage(0);
                result = 1;
                wasHandled = true;
                break;
            }
        }

        if (!wasHandled)
        {
            result = DefWindowProc(hwnd, message, wParam, lParam);
        }
    }

    return result;
}

HRESULT TurtleApp::LoadBitmapFromFile
(
    ID2D1RenderTarget *pRenderTarget,
    IWICImagingFactory *pIWICFactory,
    PCWSTR uri,
    UINT destinationWidth,
    UINT destinationHeight,
    ID2D1Bitmap **ppBitmap
)
{
    IWICBitmapDecoder *pDecoder = NULL;
    IWICBitmapFrameDecode *pSource = NULL;
    IWICStream *pStream = NULL;
    IWICFormatConverter *pConverter = NULL;
    IWICBitmapScaler *pScaler = NULL;

    HRESULT hr = pIWICFactory->CreateDecoderFromFilename
	(
        uri,
        NULL,
        GENERIC_READ,
        WICDecodeMetadataCacheOnLoad,
        &pDecoder
    );
        
    if (SUCCEEDED(hr))
    {
        hr = pDecoder->GetFrame(0, &pSource);
    }
    
	if (SUCCEEDED(hr))
    {
		hr = pIWICFactory->CreateFormatConverter(&pConverter);
    }
 
    if (SUCCEEDED(hr))
    {
        if (destinationWidth != 0 || destinationHeight != 0)
        {
            UINT originalWidth, originalHeight;
            hr = pSource->GetSize(&originalWidth, &originalHeight);
            if (SUCCEEDED(hr))
            {
                if (destinationWidth == 0)
                {
                    FLOAT scalar = static_cast<FLOAT>(destinationHeight) / static_cast<FLOAT>(originalHeight);
                    destinationWidth = static_cast<UINT>(scalar * static_cast<FLOAT>(originalWidth));
                }
                else if (destinationHeight == 0)
                {
                    FLOAT scalar = static_cast<FLOAT>(destinationWidth) / static_cast<FLOAT>(originalWidth);
                    destinationHeight = static_cast<UINT>(scalar * static_cast<FLOAT>(originalHeight));
                }

                hr = pIWICFactory->CreateBitmapScaler(&pScaler);
                if (SUCCEEDED(hr))
                {
                    hr = pScaler->Initialize
						(
                            pSource,
                            destinationWidth,
                            destinationHeight,
                            WICBitmapInterpolationModeCubic
                        );
                }

                if (SUCCEEDED(hr))
                {
                    hr = pConverter->Initialize
						(
							pScaler,
							GUID_WICPixelFormat32bppPBGRA,
							WICBitmapDitherTypeNone,
							NULL,
							0.f,
							WICBitmapPaletteTypeMedianCut
                        );
                }
            }
        }
        else
        {
            hr = pConverter->Initialize
				(
					pSource,
					GUID_WICPixelFormat32bppPBGRA,
					WICBitmapDitherTypeNone,
					NULL,
					0.f,
					WICBitmapPaletteTypeMedianCut
                );
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = pRenderTarget->CreateBitmapFromWicBitmap
			(
				pConverter,
				NULL,
				ppBitmap
            );
    }

    SafeRelease(&pDecoder);
    SafeRelease(&pSource);
    SafeRelease(&pStream);
    SafeRelease(&pConverter);
    SafeRelease(&pScaler);

    return hr;
}

void TurtleApp::RenderTurtleImage(float x, float y)
{
	D2D1_SIZE_F size = m_pBitmap->GetSize();

	float x0 = x - size.width  / 2.0f, x1 = x0 + size.width;
	float y0 = y - size.height / 2.0f, y1 = y0 + size.height;

	m_pRenderTarget->DrawBitmap(m_pBitmap, D2D1::RectF(x0, y0, x1, y1));
}