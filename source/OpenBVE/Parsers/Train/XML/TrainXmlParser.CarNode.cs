﻿using System.Xml;
using OpenBveApi.Math;
using System.Linq;
using System;
using System.Text;

namespace OpenBve.Parsers.Train
{
	partial class TrainXmlParser
	{
		private static void ParseCarNode(XmlNode Node, string fileName, int Car, ref TrainManager.Train Train, ref ObjectManager.UnifiedObject[] CarObjects, ref ObjectManager.UnifiedObject[] BogieObjects)
		{
			double driverZ = 0.0;
			string interiorFile = string.Empty;
			foreach (XmlNode c in Node.ChildNodes)
			{
				//Note: Don't use the short-circuiting operator, as otherwise we need another if
				switch (c.Name.ToLowerInvariant())
				{
					case "brake":
						Train.Cars[Car].Specs.AirBrake.Type = TrainManager.AirBrakeType.Auxillary;
						if (c.ChildNodes.OfType<XmlElement>().Any())
						{
							ParseBrakeNode(c, fileName, Car, ref Train);
						}
						else if (!String.IsNullOrEmpty(c.InnerText))
						{
							try
							{
								string childFile = OpenBveApi.Path.CombineFile(currentPath, c.InnerText);
								XmlDocument childXML = new XmlDocument();
								childXML.Load(childFile);
								XmlNodeList childNodes = childXML.DocumentElement.SelectNodes("/openBVE/Brake");
								//We need to save and restore the current path to make relative paths within the child file work correctly
								string savedPath = currentPath;
								currentPath = System.IO.Path.GetDirectoryName(childFile);
								ParseBrakeNode(childNodes[0], fileName, Car, ref Train);
								currentPath = savedPath;
							}
							catch
							{
								Interface.AddMessage(Interface.MessageType.Error, false, "Failed to load the child Brake XML file specified in " +c.InnerText);
							}
						}
						break;
					case "length":
						double l;
						if (!NumberFormats.TryParseDoubleVb6(c.InnerText, out l) | l <= 0.0)
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid length defined for Car " + Car + " in XML file " + fileName);
							break;
						}
						Train.Cars[Car].Length = l;
						break;
					case "width":
						double w;
						if (!NumberFormats.TryParseDoubleVb6(c.InnerText, out w) | w <= 0.0)
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid width defined for Car " + Car + " in XML file " + fileName);
							break;
						}
						Train.Cars[Car].Width = w;
						break;
					case "height":
						double h;
						if (!NumberFormats.TryParseDoubleVb6(c.InnerText, out h) | h <= 0.0)
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid height defined for Car " + Car + " in XML file " + fileName);
							break;
						}
						Train.Cars[Car].Height = h;
						break;
					case "motorcar":
						if (c.InnerText.ToLowerInvariant() == "1" || c.InnerText.ToLowerInvariant() == "true")
						{
							Train.Cars[Car].Specs.IsMotorCar = true;
							Train.Cars[Car].Specs.AirBrake.Type = TrainManager.AirBrakeType.Main;
						}
						else
						{
							Train.Cars[Car].Specs.IsMotorCar = false;
							Train.Cars[Car].Specs.AirBrake.Type = TrainManager.AirBrakeType.Auxillary;
						}
						break;
					case "mass":
						double m;
						if (!NumberFormats.TryParseDoubleVb6(c.InnerText, out m) | m <= 0.0)
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid mass defined for Car " + Car + " in XML file " + fileName);
							break;
						}
						Train.Cars[Car].Specs.MassEmpty = m;
						Train.Cars[Car].Specs.MassCurrent = m;
						break;
					case "frontaxle":
						if (!NumberFormats.TryParseDoubleVb6(c.InnerText, out Train.Cars[Car].FrontAxle.Position))
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid front axle position defined for Car " + Car + " in XML file " + fileName);
						}
						break;
					case "rearaxle":
						if (!NumberFormats.TryParseDoubleVb6(c.InnerText, out Train.Cars[Car].RearAxle.Position))
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid rear axle position defined for Car " + Car + " in XML file " + fileName);
						}
						break;
					case "object":
						string f = OpenBveApi.Path.CombineFile(currentPath, c.InnerText);
						if (System.IO.File.Exists(f))
						{
							CarObjects[Car] = ObjectManager.LoadObject(f, System.Text.Encoding.Default, ObjectManager.ObjectLoadMode.Normal, false, false, false);
						}
						break;
					case "reversed":
						if (c.InnerText.ToLowerInvariant() == "1" || c.InnerText.ToLowerInvariant() == "true")
						{
							CarObjectsReversed[Car] = true;
						}
						break;
					case "frontbogie":
						if (c.ChildNodes.OfType<XmlElement>().Any())
						{
							foreach (XmlNode cc in c.ChildNodes)
							{
								switch (cc.Name.ToLowerInvariant())
								{
									case "frontaxle":
										if (!NumberFormats.TryParseDoubleVb6(cc.InnerText, out Train.Cars[Car].FrontBogie.FrontAxle.Position))
										{
											Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid front bogie, front axle position defined for Car " + Car + " in XML file " + fileName);
										}
										break;
									case "rearaxle":
										if (!NumberFormats.TryParseDoubleVb6(cc.InnerText, out Train.Cars[Car].FrontBogie.RearAxle.Position))
										{
											Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid front bogie, rear axle position defined for Car " + Car + " in XML file " + fileName);
										}
										break;
									case "object":
										string fb = OpenBveApi.Path.CombineFile(currentPath, cc.InnerText);
										if (System.IO.File.Exists(fb))
										{
											BogieObjects[Car * 2] = ObjectManager.LoadObject(fb, System.Text.Encoding.Default, ObjectManager.ObjectLoadMode.Normal, false, false, false);
										}
										break;
									case "reversed":
										BogieObjectsReversed[Car * 2] = true;
										break;
								}
							}
						}
						break;
					case "rearbogie":
						if (c.ChildNodes.OfType<XmlElement>().Any())
						{
							foreach (XmlNode cc in c.ChildNodes)
							{
								switch (cc.Name.ToLowerInvariant())
								{
									case "frontaxle":
										if (!NumberFormats.TryParseDoubleVb6(cc.InnerText, out Train.Cars[Car].RearBogie.FrontAxle.Position))
										{
											Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid rear bogie, front axle position defined for Car " + Car + " in XML file " + fileName);
										}
										break;
									case "rearaxle":
										if (!NumberFormats.TryParseDoubleVb6(cc.InnerText, out Train.Cars[Car].RearBogie.RearAxle.Position))
										{
											Interface.AddMessage(Interface.MessageType.Warning, false, "Invalid rear bogie, rear axle position defined for Car " + Car + " in XML file " + fileName);
										}
										break;
									case "object":
										string fb = OpenBveApi.Path.CombineFile(currentPath, cc.InnerText);
										if (System.IO.File.Exists(fb))
										{
											BogieObjects[Car * 2 + 1] = ObjectManager.LoadObject(fb, System.Text.Encoding.Default, ObjectManager.ObjectLoadMode.Normal, false, false, false);
										}
										break;
									case "reversed":
										BogieObjectsReversed[Car * 2 + 1] = true;
										break;
								}
							}
						}
						break;
					case "driverposition":
						string[] splitText = c.InnerText.Split(',');
						if (splitText.Length != 3)
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Driver position must have three arguments for Car " + Car + " in XML file " + fileName);
							break;
						}
						Train.Cars[Car].Driver = new Vector3();
						if (!NumberFormats.TryParseDoubleVb6(splitText[0], out Train.Cars[Car].Driver.X))
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Driver position X was invalid for Car " + Car + " in XML file " + fileName);
						}
						if (!NumberFormats.TryParseDoubleVb6(splitText[1], out Train.Cars[Car].Driver.Y))
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Driver position Y was invalid for Car " + Car + " in XML file " + fileName);
						}
						if (!NumberFormats.TryParseDoubleVb6(splitText[2], out driverZ))
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Driver position X was invalid for Car " + Car + " in XML file " + fileName);
						}
						Train.Cars[Car].Driver.Z = 0.5 * Train.Cars[Car].Length + driverZ;
						break;
					case "interiorview":
						if (Train != TrainManager.PlayerTrain)
						{
							break;
						}
						Train.Cars[Car].HasInteriorView = true;
						if (Car != Train.DriverCar)
						{
							Train.Cars[Car].CarSections = new TrainManager.CarSection[1];
							Train.Cars[Car].CarSections[0] = new TrainManager.CarSection();
							Train.Cars[Car].CarSections[0].Elements = new ObjectManager.AnimatedObject[] { };
							Train.Cars[Car].CarSections[0].Overlay = true;
						}
						string cv = OpenBveApi.Path.CombineFile(currentPath, c.InnerText);
						if (!System.IO.File.Exists(cv))
						{
							Interface.AddMessage(Interface.MessageType.Warning, false, "Interior view file was invalid for Car " + Car + " in XML file " + fileName);
							break;
						}
						interiorFile = cv;
						break;
				}
			}
			//Driver position is measured from the front of the car
			//As there is no set order, this needs to be done after the loop
			if (interiorFile != String.Empty)
			{
				
				if (interiorFile.ToLowerInvariant().EndsWith(".cfg"))
				{
					//Only supports panel2.cfg format
					Panel2CfgParser.ParsePanel2Config(System.IO.Path.GetFileName(interiorFile), System.IO.Path.GetDirectoryName(interiorFile), Encoding.UTF8, Train, Car);
					Train.Cars[Car].CameraRestrictionMode = Camera.RestrictionMode.On;
				}
				else if (interiorFile.ToLowerInvariant().EndsWith(".animated"))
				{
					ObjectManager.AnimatedObjectCollection a = AnimatedObjectParser.ReadObject(interiorFile, Encoding.UTF8, ObjectManager.ObjectLoadMode.DontAllowUnloadOfTextures);
					try
					{
						for (int i = 0; i < a.Objects.Length; i++)
						{
							a.Objects[i].ObjectIndex = ObjectManager.CreateDynamicObject();
						}
						Train.Cars[Car].CarSections[0].Elements = a.Objects;
						Train.Cars[Car].CameraRestrictionMode = Camera.RestrictionMode.NotAvailable;
					}
					catch
					{
						Program.RestartArguments = " ";
						Loading.Cancel = true;
					}
				}
				else
				{
					Interface.AddMessage(Interface.MessageType.Warning, false, "Interior view file is not supported for Car " + Car + " in XML file " + fileName);
				}
			}

			
		}
	}
}