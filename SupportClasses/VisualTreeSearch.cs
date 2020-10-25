using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Niero.SupportClasses
{
    static class VisualTreeSearch
    {
        public static bool IsHadChildrenType(DependencyObject parent, Type childType)
        {
            if (parent != null && childType != null)
            {
                if (VisualTreeHelper.GetChildrenCount(parent) != 0)
                {
                    List<bool> ReturnsList = new List<bool>();
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                    {
                        if (VisualTreeHelper.GetChild(parent, i).GetType() == childType)
                        {
                            return true;
                        }
                        else
                        {
                            ReturnsList.Add(IsHadChildrenType(VisualTreeHelper.GetChild(parent, i), childType));
                        }
                    }
                    if (ReturnsList.Contains(true))
                    {
                        return true;
                    }
                }
            }
            return false;
        } //returns true if  childType founded in parent VisualTree 
        public static List<DependencyObject> SearchChildren(DependencyObject parent, Type childType)
        {
            if (parent != null && childType != null)
            {
                if (VisualTreeHelper.GetChildrenCount(parent) != 0)
                {
                    List<DependencyObject> MyResultObjectsList = new List<DependencyObject>();
                    List<List<DependencyObject>> ReturnsObjectsLists = new List<List<DependencyObject>>();
                    for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                    {
                        if (VisualTreeHelper.GetChild(parent, i).GetType() == childType)
                        {
                            MyResultObjectsList.Add(VisualTreeHelper.GetChild(parent, i));
                        }
                        else
                        {
                            ReturnsObjectsLists.Add(SearchChildren(VisualTreeHelper.GetChild(parent, i), childType));
                        }
                    }
                    foreach (List<DependencyObject> ObjectList in ReturnsObjectsLists)
                    {
                        if (ObjectList != null)
                        {
                            MyResultObjectsList.AddRange(ObjectList);
                        }
                    }
                    if(MyResultObjectsList.Count != 0)
                    {
                        return MyResultObjectsList;
                    }
                }
            }
            return null;
        } //returns all founded object of childType in parent VisualTree
    }
}
