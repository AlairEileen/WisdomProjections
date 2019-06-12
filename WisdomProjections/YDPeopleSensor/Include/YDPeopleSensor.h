
#ifndef IllllIIIIIlIl
#define IllllIIIIIlIl
#include <vector>
#include <YDPeopleSensorC.h>
namespace lllIIllII{typedef IllIIlIlIll FLOAT2;typedef IIIlllIlIIlI lllIlIlII;
typedef llIIIIIIIll FLOAT4;typedef IIIIIIIIlIII lllllIlIIll;typedef lIIIllIIll 
lIIlIllIlIl;typedef llIlllIIlIII IlIIlll;typedef lIllllllIIll lIIlIlII;typedef 
lIIIIllIlIl lIIllIIII;typedef IIlIIlIIIll IlIIIIIIII;typedef IIIIIlllIll 
lIIIllllll;typedef llllIlIIIlI IllIIlllII;const int IlllIIIIllI=IIIIIllIlllI;
enum class IIIIlI{lIlllll=IlllIlIIlII,IlIIIIlIIllI=lIlllllll,llIllllIlI=
llllIllIllII,IIIIlIlIlIII=llIllllIlIIlI,lllllIlIllII=lllIlllIllllI,lIIIIIIlII=
lIIlIIllIIllI,IIlllIIIlI=IlIlIlllIIlll,IlIIIllllII=IlIlllIIllIll,Timeout=
lllIlIIlIIIlI};enum class lllllII{Head=lllIIIlIIlIIl,llIlll=llIIIlIllllIl,
lIlIIlI=IlllllIIIIIIl,lIllIll=IllIlIlIllll,lIlIIIll=IIIllIIIIIllI,lIlIIlII=
IIllIIlIIlIlI,IlIIIllI=IIIllIIllllIl,IllIIll=IlIlIlIIIIIl,IIllllII=llIIIIIIIIlI,
IIIIIIll=IllIIIllIIIIl,IllllIl=llIlllIllllII,IIIllIl=lIlIIlIIIllII,lllllIIl=
lIIllIIlIlIlI,IIIlIlll=llIIllllIIIlI,llllllII=IlIlIlIlIIIl,lIIIIlII=
lIIIIIlIlIIlI,llllIIllI=IIlllIIIllII,Count=lIIIlIllllIl};enum class lIIIIIllll{
IIIIIllIlI=llIIllIlIllI,lIllIlIllI=IIlIIlllllll,lIllllllIl=IIIIIIllIIlII};enum 
class lIIlIIlIlI{IIIIIllIlI=llIIllIlIllI,lIllIlIllI=IIlIIlllllll};class 
IIIlIllIl{public:IIIlIllIl(){CreatePeopleSensor(&this->llIIIl);}IIIlIllIl(const 
IIIlIllIl&other)=delete;~IIIlIllIl(){DestroyPeopleSensor(this->llIIIl);}int 
Initialize(lIIIIIllll llIIllIlIl,lIIlIIlIlI IIlllIlIII,bool IIllllllIII,unsigned
 int IIllIllIlI=(0x1597+4347-0x2692)){return InitializePeopleSensor(this->llIIIl
,(lllIlIlllll)llIIllIlIl,(lllIlIlllll)IIlllIlIII,IIllllllIII,IIllIllIlI);}void 
lllIlIIIl(){UninitializePeopleSensor(this->llIIIl);}int Start(){return 
StartPeopleSensor(this->llIIIl);}void Stop(){StopPeopleSensor(this->llIIIl);}
bool IsRunning()const{return IsPeopleSensorRunning(this->llIIIl)?true:false;}
void IlIIlIlIlIl(bool IIlIlIllll){SetPeopleSensorDepthMappedToColor(this->llIIIl
,IIlIlIllll);}bool IIlIllllIll()const{return IsPeopleSensorDepthMappedToColor(
this->llIIIl)?true:false;}void lllIllIIllI(bool IIlIlIllll){
SetPeopleSensorSkeletonMappedToColor(this->llIIIl,IIlIlIllll?llIIllllllII:
IIIlIIlIIIl);}bool IllIlIIlIII()const{return IsPeopleSensorSkeletonMappedToColor
(this->llIIIl)?true:false;}int llllIlIlllI(){return 
TurnOnPeopleSensorInfraredEmitter(this->llIIIl);}int IIIIllIIllI(){return 
TurnOffPeopleSensorInfraredEmitter(this->llIIIl);}int lIIIIllIIl(){return 
EnablePeopleSensorColor(this->llIIIl);}int llIIIIllIl(){return 
DisablePeopleSensorColor(this->llIIIl);}bool IIIIIlIlII()const{return 
IsPeopleSensorColorEnabled(this->llIIIl)?true:false;}int IlIlIIlllI(){return 
EnablePeopleSensorAudio(this->llIIIl);}int IllIllllll(){return 
DisablePeopleSensorAudio(this->llIIIl);}bool llIIIIlIIll()const{return 
IsPeopleSensorAudioEnabled(this->llIIIl)?true:false;}int IllIllIlIl(bool 
llIlIllII){SetPeopleSensorNearMode(this->llIIIl,llIlIllII?llIIllllllII:
IIIlIIlIIIl);}bool llIlIllII()const{return IsPeopleSensorNearMode(this->llIIIl)?
true:false;}int llIlllIIII(IlIIIIIIII&frame){return GetPeopleSensorColorFrame(
this->llIIIl,&frame);}int llIllIllII(lIIIllllll&frame){return 
GetPeopleSensorDepthFrame(this->llIIIl,&frame);}int lIllIIIlll(IllIIlllII&frame)
{return GetPeopleSensorAudioFrame(this->llIIIl,&frame);}int lIlllllIlIl(
lIIllIIII&data){return GetPeopleSensorPublishData(this->llIIIl,&data);}int 
llIlllIIII(IlIIIIIIII&frame,unsigned int timeout){return 
GetPeopleSensorColorFrameTimeout(this->llIIIl,&frame,timeout);}int llIllIllII(
lIIIllllll&frame,unsigned int timeout){return GetPeopleSensorDepthFrameTimeout(
this->llIIIl,&frame,timeout);}int lIllIIIlll(IllIIlllII&frame,unsigned int 
timeout){return GetPeopleSensorAudioFrameTimeout(this->llIIIl,&frame,timeout);}
int lIlllllIlIl(lIIllIIII&data,unsigned int timeout){return 
GetPeopleSensorPublishDataTimeout(this->llIIIl,&data,timeout);}int lllIIlllIIIl(
const FLOAT4&IIlllIllI,FLOAT2&IlIllIIII){return 
PeopleSensorDepthSpacePointToScreen(this->llIIIl,&IIlllIllI,&IlIllIIII);}int 
IllIlIIIlIl(const FLOAT4&IIlllIllI,FLOAT2&IlIllIIII){return 
PeopleSensorColorSpacePointToScreen(this->llIIIl,&IIlllIllI,&IlIllIIII);}int 
IlIIlIllII(const IlIIIIIIII&IllIIllllI,const lIIllIIII&IllIlIlI,std::vector<
unsigned int>&IIIllIIlII){IIIllIIlII.resize(IllIIllllI.Width*IllIIllllI.llIlIl);
return PeopleSensorExtractUsers(this->llIIIl,IllIIllllI.IIIlIlIII,IllIIllllI.
Width,IllIIllllI.llIlIl,IllIlIlI.lIIlIlII.Mask,IllIlIlI.lIIlIlII.Width,IllIlIlI.
lIIlIlII.llIlIl,IllIlIlI.IlIIlll.Data,IllIlIlI.IlIIlll.Size,(IIIllIIlII.size()?&
IIIllIIlII[(0xbe3+1677-0x1270)]:nullptr));}int lllllIlllll(const lIIIllllll&
frame,lllIlIlII*lIIII)const{return ConvertPeopleSensorDepthFrameToPointCloud(
this->llIIIl,frame.Width,frame.llIlIl,frame.IIIlIlIII,(float*)lIIII);}static int
 lIIlllIIII(unsigned int&count){return GetPeopleSensorCount(&count);}IIIlIllIl&
operator=(const IIIlIllIl&other)=delete;private:llIIIIll llIIIl;};}
#endif

