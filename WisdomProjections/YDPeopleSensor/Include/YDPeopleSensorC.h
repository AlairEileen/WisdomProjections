
#ifndef llIIIIlIlllIl
#define llIIIIlIlllIl
#undef DLLFUNC
#ifdef _MSC_VER
#ifdef YDPEOPLESENSORDLL
#define DLLFUNC __declspec(dllexport)
#else
#define DLLFUNC
#endif
#else
#ifdef YDPEOPLESENSORDLL
#define DLLFUNC __attribute__ ((visibility ("\x64\x65\x66\x61\x75\x6c\x74")))
#else
#define DLLFUNC
#endif
#endif
#undef _CDECL_
#if defined(_MSC_VER) && defined(__cplusplus)
#define _CDECL_ __cdecl
#else
#define _CDECL_
#endif
#define IIIIIllIlllI ((0x1d99+1764-0x2477))
typedef void*llIIIIll;typedef unsigned char lIIlIIlll;
#define llIIllllllII  ((0xe1b+5328-0x22ea))
#define IIIlIIlIIIl ((0xa31+335-0xb80))
typedef enum IlIIlIIlIIIIlI{IlllIlIIlII=(0x21f+8062-0x219d),lIlllllll=
(0xe17+1653-0x148b),llllIllIllII=(0xf69+134-0xf8b),llIllllIlIIlI,lllIlllIllllI,
lIIlIIllIIllI,IlIlIlllIIlll,IlIlllIIllIll,lllIlIIlIIIlI}llIlIIlIIlIlI;typedef 
enum llIIIlllllllIl{lllIIIlIIlIIl=(0xc83+2740-0x1737),llIIIlIllllIl,
IlllllIIIIIIl,IllIlIlIllll,IIIllIIIIIllI,IIllIIlIIlIlI,IIIllIIllllIl,
IlIlIlIIIIIl,llIIIIIIIIlI,IllIIIllIIIIl,llIlllIllllII,lIlIIlIIIllII,
lIIllIIlIlIlI,llIIllllIIIlI,IlIlIlIlIIIl,lIIIIIlIlIIlI,IIlllIIIllII,lIIIlIllllIl
}IIllIllIlIllIl;typedef enum IIIIIlllllllll{llIIllIlIllI,IIlIIlllllll,
IIIIIIllIIlII}lllIlIlllll;typedef struct IllllIlIIIlIll{float x;float y;}
IllIIlIlIll;typedef struct llIlIIlIlIllll{float x;float y;float Illll;}
IIIlllIlIIlI;typedef struct llIIlIIIllllll{float x;float y;float Illll;float 
IlIlll;}llIIIIIIIll;typedef struct llIlllIIlIIlII{llIIIIIIIll Position;
llIIIIIIIll IlIIllll;IIIlllIlIIlI IIllIIllI[(0x13a4+1194-0x184b)];}IIIIIIIIlIII;
typedef struct lIlllIlIlIllll{unsigned short UserID;lIIlIIlll IIIlllIllI;
llIIIIIIIll Position;IIIIIIIIlIII lIlIllII[lIIIlIllllIl];}lIIIllIIll;typedef 
struct IIIIIIIIIlllII{unsigned int Size;lIIIllIIll*Data;}llIlllIIlIII;typedef 
struct lIIlIIIlIIIlI{unsigned short*Mask;unsigned int Width;unsigned int llIlIl;
}lIllllllIIll;typedef struct llIlIIIIlIllII{llIlllIIlIII IlIIlll;llIIIIIIIll 
llIllIlll;lIllllllIIll lIIlIlII;long long IlIlIII;}lIIIIllIlIl;typedef struct 
lIlIllIlIIIllI{unsigned int Width;unsigned int llIlIl;long llIlIIIll;long long 
IlIlIII;const unsigned int*IIIlIlIII;}IIlIIlIIIll;typedef struct IIIIlllIllllII{
unsigned int Width;unsigned int llIlIl;long llIlIIIll;long long IlIlIII;const 
unsigned short*IIIlIlIII;}IIIIIlllIll;typedef struct IlIlIIIlIlIIlI{unsigned int
 Size;unsigned int Duration;long llIlIIIll;long long IlIlIII;const unsigned 
short*Data;}llllIlIIIlI;
#ifdef __cplusplus
extern"C"{
#endif
DLLFUNC int _CDECL_ GetPeopleSensorCount(unsigned int*IlIIlllllIll);DLLFUNC 
lIIlIIlll _CDECL_ CreatePeopleSensor(llIIIIll*lIIlIIIllll);DLLFUNC void _CDECL_ 
DestroyPeopleSensor(llIIIIll llIIIl);DLLFUNC int _CDECL_ InitializePeopleSensor(
llIIIIll llIIIl,lllIlIlllll llIIllIlIl,lllIlIlllll IIlllIlIII,lIIlIIlll 
IIllllllIII,unsigned int IIllIllIlI);DLLFUNC void _CDECL_ 
UninitializePeopleSensor(llIIIIll llIIIl);DLLFUNC int _CDECL_ StartPeopleSensor(
llIIIIll llIIIl);DLLFUNC void _CDECL_ StopPeopleSensor(llIIIIll llIIIl);DLLFUNC 
lIIlIIlll _CDECL_ IsPeopleSensorRunning(llIIIIll llIIIl);DLLFUNC void _CDECL_ 
SetPeopleSensorDepthMappedToColor(llIIIIll llIIIl,lIIlIIlll IIlIlIllll);DLLFUNC 
lIIlIIlll _CDECL_ IsPeopleSensorDepthMappedToColor(llIIIIll llIIIl);DLLFUNC void
 _CDECL_ SetPeopleSensorSkeletonMappedToColor(llIIIIll llIIIl,lIIlIIlll 
IIlIlIllll);DLLFUNC lIIlIIlll _CDECL_ IsPeopleSensorSkeletonMappedToColor(
llIIIIll llIIIl);DLLFUNC int _CDECL_ TurnOnPeopleSensorInfraredEmitter(llIIIIll 
llIIIl);DLLFUNC int _CDECL_ TurnOffPeopleSensorInfraredEmitter(llIIIIll llIIIl);
DLLFUNC int _CDECL_ EnablePeopleSensorColor(llIIIIll llIIIl);DLLFUNC int _CDECL_
 DisablePeopleSensorColor(llIIIIll llIIIl);DLLFUNC lIIlIIlll _CDECL_ 
IsPeopleSensorColorEnabled(llIIIIll llIIIl);DLLFUNC int _CDECL_ 
EnablePeopleSensorAudio(llIIIIll llIIIl);DLLFUNC int _CDECL_ 
DisablePeopleSensorAudio(llIIIIll llIIIl);DLLFUNC lIIlIIlll _CDECL_ 
IsPeopleSensorAudioEnabled(llIIIIll llIIIl);DLLFUNC int _CDECL_ 
SetPeopleSensorNearMode(llIIIIll llIIIl,lIIlIIlll llIlIllII);DLLFUNC lIIlIIlll 
_CDECL_ IsPeopleSensorNearMode(llIIIIll llIIIl);DLLFUNC int _CDECL_ 
GetPeopleSensorColorFrame(llIIIIll llIIIl,IIlIIlIIIll*frame);DLLFUNC int _CDECL_
 GetPeopleSensorDepthFrame(llIIIIll llIIIl,IIIIIlllIll*frame);DLLFUNC int 
_CDECL_ GetPeopleSensorAudioFrame(llIIIIll llIIIl,llllIlIIIlI*frame);DLLFUNC int
 _CDECL_ GetPeopleSensorPublishData(llIIIIll llIIIl,lIIIIllIlIl*data);DLLFUNC 
int _CDECL_ GetPeopleSensorColorFrameTimeout(llIIIIll llIIIl,IIlIIlIIIll*frame,
unsigned int timeout);DLLFUNC int _CDECL_ GetPeopleSensorDepthFrameTimeout(
llIIIIll llIIIl,IIIIIlllIll*frame,unsigned int timeout);DLLFUNC int _CDECL_ 
GetPeopleSensorAudioFrameTimeout(llIIIIll llIIIl,llllIlIIIlI*frame,unsigned int 
timeout);DLLFUNC int _CDECL_ GetPeopleSensorPublishDataTimeout(llIIIIll llIIIl,
lIIIIllIlIl*data,unsigned int timeout);DLLFUNC int _CDECL_ 
PeopleSensorDepthSpacePointToScreen(llIIIIll llIIIl,const llIIIIIIIll*IIlllIllI,
IllIIlIlIll*IlIllIIII);DLLFUNC int _CDECL_ PeopleSensorColorSpacePointToScreen(
llIIIIll llIIIl,const llIIIIIIIll*IIlllIllI,IllIIlIlIll*IlIllIIII);DLLFUNC int 
_CDECL_ PeopleSensorExtractUsers(llIIIIll llIIIl,const unsigned int*lIIIIlll,int
 IIlIIIllI,int IlIlllIlIl,const unsigned short*mask,int llIIlllIl,int IIIlIIllIl
,const lIIIllIIll*IlIlIllll,int llllIlIIlII,unsigned int*IIIllIIlII);DLLFUNC int
 _CDECL_ ConvertPeopleSensorDepthFrameToPointCloud(llIIIIll llIIIl,int width,int
 IlllI,const unsigned short*depth,float*lIIII);DLLFUNC int _CDECL_ YDGreenScreen
(const unsigned int*lIIIIlll,unsigned int IllIIllIlI,unsigned int lIIIIllIll,
const unsigned short*mask,unsigned int lllIlIllll,unsigned int IllIIllIIl,
unsigned int*lIIIlIlIlIl);
#ifdef __cplusplus
}
#endif
#undef _CDECL_
#undef DLLFUNC
#endif

