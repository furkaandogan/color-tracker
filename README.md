# color-tracker
2010 yılında aforget.net kütüphanesini kullanılarak kameradan alınan görüntüdeki istenilen nesneyi takip eden ve konum bilgilerini veren proje


## Properties
- [.ScreenWidth](#screenwidth)
- [.ScreenHeight](#screenheight)
- [.WebCamList](#webcamlist)
- [.SelectedWebCam](#selectedwebcam)
- [.FilterColorRGB](#filtercolorrgb)
- [.Frame](#frame)
- [.FrameSize](#framesize)
- [.Instance](#instance)

## Events
- [.OnTracking](#ontracking)
- [.OnRenderFrame](#onrenderframe)


## Methods
- [.OpenCam](#opencam)
- [.Tracke](#tracke)
- [.GetWebCams](#getwebcams)
- [.Dispose](#dispose)

## Overview

```csharp
Bin.ColorTracking colorTracking=new Bin.ColorTracking();
```

### .ScreenWidth
Geçerli ekranın genişliğini verir.
```csharp
 int width = Bin.ColorTracking.ScreenWidth;
 // width => 1920
```

### .ScreenHeight
Geçerli ekranın yüksekliğini verir.
```csharp
 int height = Bin.ColorTracking.ScreenHeight;
 // height => 1920
```

### .WebCamList
Sınıf oluşturulurken bağlı bulunan kamerların bilgilerini içerir.
```csharp
 List<WebCam> webCamList = colorTracking.WebCamList;
 // [{Name:"Snoy Cam",..}...]
```


### .FilterColorRGB
Frame içerisinde aranan objenin renk kodu
```csharp
 colorTracking.FilterColorRGB=new RGB(Color.Green);
```

### .Frame
Webcamnin saniyede alacağı görüntü sayısı varsayılan değer 30 fps
```csharp
 colorTracking.Frame=60;
```

### .FrameSize
Webcamden alınacak görüntünün çıkyı boyutu (1920x1080) varsayılan değer geçerli ekran çözünürlüğü

```csharp
 colorTracking.FrameSize=new Size(1920,1080);
```


### .Instance
Uygulama içerisinde oluşturulan son tracker instancenı getirir eğer oluşturulmammış ise varsayılan değerlerle oluşturup döner

```csharp
Bin.ColorTracking currentColorTracking=Bin.ColorTracking.Instance;
```


### .OnTracking
Herbir frame için aranan bulunan nesnenin bilgilerini gönderen event

```csharp
Bin.ColorTracking colorTracking=new Bin.ColorTracking();

colorTracking.OnTracking += ColorTracking_OnTracking;

private void ColorTracking_OnTracking(Rectangle findObject, EventArgs e)
{
    Cursor.Position = new System.Drawing.Point(findObject.X + (findObject.Width / 2), findObject.Y + (findObject.Height / 2));
}
 
```

### .OnRenderFrame
Kameradan alınan her bir için çalışan eventtir. Alınan görüntü gönderilir.

```csharp
Bin.ColorTracking colorTracking=new Bin.ColorTracking();

colorTracking.OnRenderFrame += ColorTracking_OnRenderFrame;

private void ColorTracking_OnRenderFrame(Bitmap bitmap, EventArgs e)
{
    pictureBox2.Image = bitmap;
}
 
```

### .OpenCam
Seçilen kameradan görüntü almayı başlatır

```csharp
Bin.ColorTracking colorTracking=new Bin.ColorTracking();
WebCam cam=colorTracking.GetWebCams().FirstOrDefault();
colorTracking.OpenCam(cam);

// or

Bin.ColorTracking colorTracking=new Bin.ColorTracking();
colorTracking.SelectedWebCam=colorTracking.GetWebCams().FirstOrDefault();
colorTracking.OpenCam();
 
```

### .Tracke
El ile verilen Bitmap nesnesindeki nesneyi bulur

```csharp
Bin.ColorTracking colorTracking=new Bin.ColorTracking();
colorTracking.FilterColorRGB=new RGB(Color.Green);
colorTracking.Tracke(new Bitmap());
 
```

### .GetWebCams

Bağlı bulunan kameralarının listesini verir

```csharp

Bin.ColorTracking colorTracking=new Bin.ColorTracking();
List<WebCam> camList=colorTracking.GetWebCams();
 
```
