using System.IO;
using System.Text;

namespace PBCLib
{
    public class PBC
    {
        public Header Header;
        public Tile[,] Tiles;

        public PBC(string filename) : this(File.ReadAllBytes(filename))
        {
        }
        public PBC(byte[] data) : this(new MemoryStream(data))
        {
        }

        public PBC(Stream stream) : this(new BinaryReader(stream))
        {
        }

        public PBC(BinaryReader reader)
        {
            Header = new Header(reader);
            Tiles = new Tile[Header.Height, Header.Width];

            for (var h = 0; h < Header.Height; h++)
            {
                for (var w = 0; w < Header.Width; w++)
                {
                    Tiles[h, w] = new Tile(reader);
                }
            }
        }
    }

    public class Header
    {
        public int Height;
        public int Width;
        public uint OffsetX;
        public uint OffsetY;

        public Header(BinaryReader reader)
        {
            var magic = reader.ReadBytes(4);
            if (Encoding.ASCII.GetString(magic) != "pbc\0")
            {
                throw new InvalidDataException("Not a pbc file");
            }

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            OffsetX = reader.ReadUInt32();
            OffsetY = reader.ReadUInt32();
        }
    }

    public class Tile
    {
        public float[,] Height1;
        public float[,] Height2;
        public float[,] Height3;

        public float[][,] Layers;

        public TileType[,] Type;

        public Tile(BinaryReader reader)
        {
            Layers = new float[4][,];
            Type = new TileType[2, 2];

            var dat = reader.ReadBytes(52 - 4); // Cheese to skip height map data for now

            /* TODO The height map layers
            for (var i = 0; i < 4; i++)
            {
                Layers[i] = new float[2, 2];

                for (var y = 0; y < 2; y++)
                {
                    for (var x = 0; x < 2; x++)
                    {
                        Layers[i][x, y] = reader.ReadSingle();
                    }
                }
            }
            */

            Type[0, 0] = (TileType)reader.ReadByte();
            Type[0, 1] = (TileType)reader.ReadByte();
            Type[1, 1] = (TileType)reader.ReadByte();
            Type[1, 0] = (TileType)reader.ReadByte();
            
        }
    }

    public enum TileType
    {
        Grass,
        River,
        SoilHard,
        Stone,
        SandyBeach,
        Sea,
        TreeThick,
        Null,
        Building,
        NoEdit,
        Door0,
        Camera,
        Bracket,
        AlternateBed,
        Landing,
        Uniform,
        SoilSoft,
        Marble,
        CoatingFloor,
        Gravel,
        FallenLeaves,
        Snow,
        TreeThin,
        Carpet,
        Bald,
        Iron,
        RoadSoil,
        RoadCobblestone,
        Scolding,
        Wavy,
        Lugs,
        RoadDark,
        RoadSand,
        RoadBrick,
        RoadTree,
        RoadTile,
        RoadCobbling,
        Imposing,
        Sponge,
        VinylSheet,
        MyDesignFloor,
        RockyPlace,
        NoEntryRocky,
        RockyWater,
        Sandstone,
        Pier,
        NoWaves,
        BuildingNoGrass,
        IndoorGrass,
        NoEditGrass,
        Mud,
        StoneGrass,
        MyDesignOutdoor,
        WaterPuddle,
        MuseumWaterSurface,
        RightRotationProhibited,
        LeftRotationProhibited,
        Disabled,
        NoSoilHardEntry,
        NoSandyBeachEntry,
        WaterSand,
        WaterGravel,
        Lava,
        IndoorSand,
        NoSeaEntry,
        DreamRoomOnly,
        Embankment,
        Custom0,
        Custom1,
        Custom2,
        SandyBeachNoFootprint,
        Custom3
    }
}
