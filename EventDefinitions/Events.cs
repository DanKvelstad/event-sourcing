using System;

namespace SmartHome
{
    
namespace Events
{

class GeneralMetaData
{
    
    public string Name;

}

class BrightnessChanged
{

    public Guid Identifier;
    
    public int Change;

}

class ColorChanged
{

    public Guid Identifier;
    
    public int Change;

}

}

}