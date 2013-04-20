#import <Foundation/Foundation.h>
//Sample provided by Fabio Galuppo
const float PI = 3.14159265f;

@interface Radians2Degree : NSObject 
{
}

-(float) Convert:(float)value;
@end


@implementation Radians2Degree
-(float) Convert:(float)value
{
    return 180 * (value / PI);
}
@end

@interface Degree2Radians : NSObject 
{
}

-(float) Convert:(float)value;
@end


@implementation Degree2Radians
-(float) Convert:(float)value
{
    return PI * (value / 180);
}
@end

float callConvert(id receiver, float value)
{
    return [receiver Convert:value]; 
}

int main(int argc, const char* argv[])
{
    NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];    
    
    Radians2Degree* r2d = [[Radians2Degree alloc] init];
    Degree2Radians* d2r = [[Degree2Radians alloc] init];
    
    NSLog(@"2PI in degrees == %f\n", [r2d Convert:(2 * PI)]);
    NSLog(@"360 in radians == %f\n", [d2r Convert:(360)]);
    NSLog(@"PI in degrees == %f\n", [r2d Convert:([d2r Convert:(180)])]);
    NSLog(@"180 in radians == %f\n", [d2r Convert:([r2d Convert:(PI)])]);
    
    NSLog(@"(1/2)PI in degrees == %f\n", callConvert(r2d, PI / 2));
    NSLog(@"90 in radians == %f\n", callConvert(d2r, 90));
    
    NSLog(@"270 in radians == %f\n", callConvert(nil, 270));
    
    [pool release];
    
    return 0;
}