interface ICoordinate
{
    latitude : number;
    longitude : number;
}

interface IDimension
{
    min : ICoordinate;
    max : ICoordinate;
}

class KeyValuePair<A, B>
{
    public readonly Key : A;
    public readonly Value : B;
    constructor(key : A, value : B){
        this.Key = key;
        this.Value = value;
    }
}

class ColorDefinition
{
    public readonly Type : string;
    public readonly Color : string;
    public readonly Width : number;

    constructor(type : string, color : string, width : number = 1){
        this.Type = type;
        this.Color = color;
        this.Width = width;
    }
}

class Coordinate implements ICoordinate
{
    readonly latitude : number;
    readonly longitude : number;

    constructor(latitude : number, longitude : number)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }
}

interface IPolyLine
{
    coordinates : Array<ICoordinate>;
    innerShapes : Array<IPolyLine>;
    type : string;
}

interface IMapDefinition
{
    maxBound : ICoordinate;
    minBound : ICoordinate;
    lines : Array<IPolyLine>;
}

class Helpers{
    /** Create a random color. But can define some constants. */
    static RandomColor(red : number = undefined, green : number = undefined, blue : number = undefined) : string {
        let r = red != undefined ? red : Math.random() * 256;
        let b = blue != undefined ? blue : Math.random() * 256;
        let g = green != undefined ? green : Math.random() * 256;
        let a = 0.4;

        return "rgba(" + r + "," + g + "," + b + "," + a + ")";
    }
}

class Http
{
    static GetAsync(url : string) : Promise<string>
    {
        return new Promise<string>((resolve, reject) => {
            let xmlReq = new XMLHttpRequest();
            xmlReq.onreadystatechange = () =>
            {
                if (xmlReq.readyState !== 4)
                {
                    return;
                }

                if (xmlReq.status === 200)
                {
                    resolve(xmlReq.responseText);
                }
                else
                {
                    console.log(`HTTP ERROR to ${url} ${xmlReq.status} = ${xmlReq.responseText}`);
                    reject();
                }
            };
            xmlReq.open('GET', url, true);
            xmlReq.send(null);
        });
    }
}


class MapDrawer
{
    private readonly canvas : HTMLCanvasElement;

    constructor(canvas : HTMLCanvasElement)
    {
        this.canvas = canvas;
    }

    private MapPoint(map : IMapDefinition, coord : ICoordinate) : ICoordinate
    {
        let cLatW = map.maxBound.latitude - map.minBound.latitude;
        let cLonW = map.maxBound.longitude - map.minBound.longitude;

        let worldSize = Math.max(cLatW, cLonW);
        let size = Math.min(this.canvas.width, this.canvas.height);

        let lat = this.canvas.height - (Math.abs(coord.latitude - map.minBound.latitude) / worldSize * size);
        let long = Math.abs(coord.longitude - map.minBound.longitude) / worldSize * size;
        return new Coordinate(lat, long);
    }

    DrawPath(poly : IPolyLine, graphics : CanvasRenderingContext2D, map : IMapDefinition)
    {
        for(let i = 0; i < poly.coordinates.length; i++)
        {
            let pnt = this.MapPoint(map, poly.coordinates[i]);

            if (i === 0)
            {
                graphics.moveTo(pnt.longitude, pnt.latitude);
            }
            else
            {
                graphics.lineTo(pnt.longitude, pnt.latitude);
            }
        }
        graphics.closePath();
        
        if (!poly.innerShapes)
        {
            return;
        }

        poly.innerShapes.forEach(inner => {
            this.DrawPath(inner, graphics, map);
        });
    }

    Draw(mapjson : IMapDefinition, color : ColorDefinition)
    {
        let graphics = this.canvas.getContext("2d");
        //graphics.fillRect(0, 0, this.canvas.width, this.canvas.height);

        graphics.fillStyle = color.Color;
        graphics.strokeStyle = color.Color;
        graphics.lineWidth = color.Width;

        mapjson.lines.filter(l => l.type.toLowerCase() === "polygon").forEach(line => {
            let first = true;

            graphics.beginPath();
            this.DrawPath(line, graphics, mapjson);
            graphics.fill('evenodd');
        });
        
        graphics.beginPath();
        mapjson.lines.filter(l => l.type.toLowerCase() === "line").forEach(line => {
            let first = true;
            line.coordinates.forEach(c => {
                let pnt = this.MapPoint(mapjson, c);

                if (first)
                {
                    graphics.moveTo(pnt.longitude, pnt.latitude);
                    first = false;
                }
                else
                {
                    graphics.lineTo(pnt.longitude, pnt.latitude);
                }
            });
        });
        
        graphics.closePath();
        graphics.stroke();
    }
}

async function loadAll(){
    let baseUrl = 'http://localhost:53056/api/Map';
    let canvas = document.getElementById("mainCanvas") as HTMLCanvasElement;
    canvas.width = canvas.parentElement.clientWidth * 3;
    canvas.height = canvas.parentElement.clientHeight * 3;
    let mapDrawer = new MapDrawer(canvas);

    let dimensionRaw = {
        "min" : {
            "latitude" : 47.4657,
            "longitude" : -122.4093000
        },
        "max" : {
            "latitude" : 47.7601,
            "longitude" : -122.087000
        }
    };
    //dimensionRaw = JSON.parse(await Http.GetAsync(`${baseUrl}/dimension`));

    let dimension = dimensionRaw as IDimension;

    let baseTypes = new Array<ColorDefinition>();
    baseTypes.push(new ColorDefinition("schools", "orange"));
    baseTypes.push(new ColorDefinition("parks", "lightgreen"));
    baseTypes.push(new ColorDefinition("water", "lightblue"));
    baseTypes.push(new ColorDefinition("highways", "red", 4));
    baseTypes.push(new ColorDefinition("roads", "black"));
    baseTypes.push(new ColorDefinition("footpaths", "gray"));
    baseTypes.push(new ColorDefinition("houses", "purple"));

    for(let i = 0; i < baseTypes.length; i++)
    {
        let type = baseTypes[i];
        let file = await Http.GetAsync(`${baseUrl}/${type.Type}?longitudeMin=${dimension.min.longitude}&longitudeMax=${dimension.max.longitude}&latitudeMin=${dimension.min.latitude}&latitudeMax=${dimension.max.latitude}`);
        let mapJson = JSON.parse(file);
        mapDrawer.Draw(mapJson, type);
    }

    let typeStr = await Http.GetAsync(`${baseUrl}/types`);
    let types = JSON.parse(typeStr);

    let defaultColorDefinition = new ColorDefinition("", "black", 1);
    types.forEach(async type => {

        if (baseTypes.findIndex(bt => bt.Type.toUpperCase() == type.toUpperCase()) !== -1)
        {
            return;
        }

        let file = await Http.GetAsync(`${baseUrl}/${type}`);
        let mapJson = JSON.parse(file);
        mapDrawer.Draw(mapJson, defaultColorDefinition);
    });
}

loadAll();