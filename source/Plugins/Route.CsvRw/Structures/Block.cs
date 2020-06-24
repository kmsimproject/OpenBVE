﻿using System.Collections.Generic;
using OpenBveApi.Routes;
using RouteManager2.Climate;

namespace CsvRwRouteParser
{
	/// <summary>A single parsed block of data from a routefile</summary>
	/// <remarks>Default block length is 25m</remarks>
	internal class Block
	{
		internal int Background;
		internal Brightness[] BrightnessChanges;
		internal Fog Fog;
		internal bool FogDefined;
		internal int[] Cycle;
		internal RailCycle[] RailCycles;
		internal double Height;
		internal Dictionary<int, Rail> Rails;
		internal int[] RailType;
		internal WallDike[] RailWall;
		internal WallDike[] RailDike;
		internal Pole[] RailPole;
		internal FreeObj[][] RailFreeObj;
		internal FreeObj[] GroundFreeObj;
		internal Form[] Forms;
		internal Crack[] Cracks;
		internal Signal[] Signals;
		internal Section[] Sections;
		internal Limit[] Limits;
		internal Stop[] StopPositions;
		internal Sound[] SoundEvents;
		internal Transponder[] Transponders;
		internal DestinationEvent[] DestinationChanges;
		internal PointOfInterest[] PointsOfInterest;
		internal TrackElement CurrentTrackState;
		internal double Pitch;
		internal double Turn;
		internal int Station;
		internal bool StationPassAlarm;
		internal double Accuracy;
		internal double AdhesionMultiplier;

		internal Block()
		{
			Rails = new Dictionary<int, Rail>();
		}
	}
}
