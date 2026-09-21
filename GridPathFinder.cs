using Engine;

namespace GridPathFinder{
    public class Mapper
    {
        public int[,] weights;
        public int X {get; private set;}
        public int Y {get; private set;}
        public int WeightestValue;
        public int Width {get;}
        public int Height {get;}
        public Mapper(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            weights = new int[height, width];
        }

        public int? TryGetWeight(int x, int y)
        {
            if (x < Width && y < Height && x >= 0 && y >= 0)
            {
                return weights[y, x];
            }
            return null;
        }

    }

    public class Grid
    {
        public int Width {get;}
        public int Height{get;}
        List<(int x, int y, int weight)> onQueie = new();
        bool[,] blockedAreas;
        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            blockedAreas = new bool[height, width];
        }

        public void SetBlockState(int x, int y, bool state)
        {
            blockedAreas[y, x] = state;
        }

        public void UpdateMapper(Mapper mapper)
        {
            ClearWeights(mapper.weights);
            GetWeightsAt(mapper.X, mapper.Y, mapper.weights, out mapper.WeightestValue);
        }

        public bool GetBlockAt(int x, int y)
        {
            return blockedAreas[y, x];
        }

        public Mapper CreateMapping(int x, int y)
        {
            var mapper = new Mapper(x, y, Width, Height);
            GetWeightsAt(x, y, mapper.weights, out mapper.WeightestValue);
            return mapper;
        }

        private int[,] GetWeightsAt(int x, int y, int[,] grid, out int weightestValue)
        {
            weightestValue = 0;
            ClearWeights(grid);
            grid[y, x] = 0;
            SetNeightboors((x, y, 0), grid, ref weightestValue);

            while(onQueie.Count > 0){
                List<(int x, int y, int weight)> referenceQueie = new(onQueie);
                onQueie.Clear();

                foreach(var next in referenceQueie)
                    SetNeightboors(next, grid, ref weightestValue);
                
                referenceQueie.Clear();
            }
            return grid;
        }

        private void SetNeightboors((int x, int y, int Weight) current, int[,] grid, ref int weightestValue)
        {
            ToQueie(0, 1, ref current, grid, ref weightestValue);
            ToQueie(0, -1, ref current, grid, ref weightestValue);
            ToQueie(1, 0, ref current, grid, ref weightestValue);
            ToQueie(-1, 0, ref current, grid, ref weightestValue);
        }

        private void ToQueie(int dirX, int dirY, ref (int x, int y, int Weight) current, int[,] grid, ref int weightestValue)
        {
            if (TryGetAvailable(current.x+dirX, current.y+dirY, grid))
            {
                (int x, int y, int weight) value = (current.x+dirX, current.y+dirY, current.Weight + 1);
                grid[value.y, value.x] = value.weight;
                if (value.weight > weightestValue) weightestValue = value.weight;
                onQueie.Add(value);
            }
        }

        public bool TryGetAvailable(int x, int y, int[,] grid)
        {
            if (x < Width && y < Height && x >= 0 && y >= 0)
            {
                int weight = grid[y, x];
                if (weight == -1) return true;
                return false;
            }
            return false;
        }

        private void ClearWeights(int[,] grid)
        {
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    grid[y, x] = blockedAreas[y, x] ? -2 : -1;
                }
            }
        }
    }

    public static class Tracker
    {
        public static List<(int x, int y)> CreateTrack(int x, int y, Mapper mapper)
        {
            List<(int x, int y)> track = new();
            Run((x, y), ref track, mapper);
            return track;
        }

        private static void Run((int x, int y) current, ref List<(int x, int y)> values, Mapper mapper)
        {
            values.Add(current);

            var neightboors = GetNeigtboors(current, mapper);
            if(neightboors.Count == 0) return;

            (int x, int y) totalCoordSum = (0, 0);

            foreach(var neigh in neightboors)
            {
                totalCoordSum.x += neigh.x - current.x;
                totalCoordSum.y += neigh.y - current.y;
            }
            totalCoordSum.x += current.x;
            totalCoordSum.y += current.y;

            if (totalCoordSum == current)
                Run(Rand.Choice(neightboors), ref values, mapper);

            else{
                var weight = mapper.TryGetWeight(totalCoordSum.x, totalCoordSum.y);
                if (weight.HasValue == false || weight.Value < 0)
                {
                    Run(Rand.Choice(neightboors), ref values, mapper);
                }

                if (weight.HasValue)
                {
                    if(weight.Value > 0)
                        Run(totalCoordSum, ref values, mapper);
                    else values.Add(totalCoordSum);
                }
            }
        }

        private static List<(int x, int y)> GetNeigtboors((int x, int y) coord, Mapper mapper)
        {
            List<(int x, int y)> surrounds = new();
            int lowest = 0;
            (int x, int y) direction = (0, 1);
            (int x, int y) value;
            for(int i = 0; i < 4; i++)
            {
                value = (coord.x + direction.x, coord.y + direction.y);
                var result = mapper.TryGetWeight(value.x, value.y);
                if (result.HasValue && result >= 0)
                {
                    if (surrounds.Count == 0)
                    {
                        surrounds.Add(value);
                        lowest = result.Value;
                    } else if (result.Value == lowest)
                    {
                        surrounds.Add(value);
                    }
                    else if (result.Value < lowest)
                    {
                        surrounds.Clear();
                        lowest = result.Value;
                        surrounds.Add(value);
                    }
                }
                direction = (-direction.y, direction.x);
            }
            return surrounds;
        }
    }
}