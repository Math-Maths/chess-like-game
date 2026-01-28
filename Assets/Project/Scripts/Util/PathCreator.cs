using UnityEngine;
using ChessGame;

public static class PathCreator
{
    public static BoardCreator.Coordinate[] CreatePath(BoardCreator.Coordinate start, BoardCreator.Coordinate end)
    {
        int deltaX = end.x - start.x;
        int deltaY = end.y - start.y;

        if(Mathf.Abs(deltaX) == 0 || Mathf.Abs(deltaY) == 0)
        {
            //straight line
            if(Mathf.Abs(deltaX) > 0)
            {
                //horizontal
                return CreateStraightPath(start, end, true);
            }
            else
            {
                //vertical
                return CreateStraightPath(start, end, false);
            }
        }
        else if(Mathf.Abs(deltaX) == Mathf.Abs(deltaY))
        {
            //diagonal
            return CreateDiagonalPath(start, end);
        }
        else
        {
            //horse like move
            return CreateHorsePath(start, end);
        }
    }

    static BoardCreator.Coordinate[] CreateHorsePath(BoardCreator.Coordinate start, BoardCreator.Coordinate end)
    {
        BoardCreator.Coordinate[] path = new BoardCreator.Coordinate[3];

        int deltaX = end.x - start.x;
        int deltaY = end.y - start.y;

        int stepX = (deltaX > 0) ? 1 : -1;
        int stepY = (deltaY > 0) ? 1 : -1;

        if(Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
        {
            //horizontal first
            BoardCreator.Coordinate firstPoint = new BoardCreator.Coordinate(start.x + 1 *stepX, start.y);
            BoardCreator.Coordinate secondPoint = new BoardCreator.Coordinate(start.x + 2 *stepX, start.y);
            BoardCreator.Coordinate lastPoint = new BoardCreator.Coordinate(end.x, end.y);

            path[0] = firstPoint;
            path[1] = secondPoint;
            path[2] = lastPoint;
        }
        else
        {
            //vertical first
            BoardCreator.Coordinate firstPoint = new BoardCreator.Coordinate(start.x, start.y + 1 * stepY);
            BoardCreator.Coordinate secondPoint = new BoardCreator.Coordinate(start.x, start.y + 2 * stepY);
            BoardCreator.Coordinate lastPoint = new BoardCreator.Coordinate(end.x, end.y);

            path[0] = firstPoint;
            path[1] = secondPoint;
            path[2] = lastPoint;
        }

        return path;
    }

    static BoardCreator.Coordinate[] CreateDiagonalPath(BoardCreator.Coordinate start, BoardCreator.Coordinate end)
    {
        int pathSize = Mathf.Abs(end.x - start.x);
        BoardCreator.Coordinate[] path = new BoardCreator.Coordinate[pathSize];

        //Step direction
        int stepX = (end.x - start.x) > 0 ? 1 : -1;
        int stepY = (end.y - start.y) > 0 ? 1 : -1;

        for(int i = 0; i < pathSize; i++)
        {
            BoardCreator.Coordinate stepCoord = new BoardCreator.Coordinate(start.x + ((i + 1) * stepX), start.y + ((i + 1) * stepY));
            path[i] = stepCoord;
        }

        return path;
    }

    static BoardCreator.Coordinate[] CreateStraightPath(BoardCreator.Coordinate start, BoardCreator.Coordinate end, bool isHorizontal)
    {
        if(isHorizontal)
        {
            int pathSize = Mathf.Abs(end.x - start.x);
            BoardCreator.Coordinate[] path = new BoardCreator.Coordinate[pathSize];

            //Step direction
            int step = (end.x - start.x) > 0 ? 1 : -1;

            for(int i = 0; i < pathSize; i++)
            {   
                BoardCreator.Coordinate stepCoord = new BoardCreator.Coordinate(start.x + ((i + 1) * step), start.y);
                path[i] = stepCoord;
            }

            return path;
        }
        else
        {
            int pathSize = Mathf.Abs(end.y - start.y);
            BoardCreator.Coordinate[] path = new BoardCreator.Coordinate[pathSize];

            //Step direction
            int step = (end.y - start.y) > 0 ? 1 : -1;

            for(int i = 0; i < pathSize; i++)
            {
                BoardCreator.Coordinate stepCoord = new BoardCreator.Coordinate(start.x, start.y + ((i + 1) * step));
                path[i] = stepCoord;
            }

            return path;
        }
    }
}