using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kraken.model
{
    [Flags]
    public enum AccessRightFlags
    {
        IsTemporary
    }

    public class User
    {
        public string name { get; set; }
    }

    public class AccessRight
    {
        public string name { set; get; }
        AccessRightFlags flags { get; set; }
    }

    public class GridAddress
    {
        List<string> bits;

        public GridAddress(string adr)
        {
            bits = adr.Split('.').ToList();
        }

        public GridAddress(List<string> bits)
        {
            this.bits = bits;
        }

        public string top()
        {
            return bits.First();
        }

        public bool topMatch(string top)
        {
            if (bits.Count > 0)
            {
                var adrTop = bits.First();
                return adrTop == top;
            }
            return false;
        }

        public bool isLeaf()
        {
            return bits.Count == 1;
        }

        public GridAddress pop()
        {
            var newBits = bits.Skip(1).ToList();
            return new GridAddress(newBits);
        }
    }

    public class AccessGrid
    {
        public string name { set; get; }
        public AccessGrid parent;
        public List<AccessGrid> subgrids;
        public Dictionary<User, List<AccessRight>> accessRights;
        public List<string> tags = new List<string>();

        public AccessGrid()
        {
            subgrids = new List<AccessGrid>();
            name = "";
            parent = null;
            accessRights = new Dictionary<User,List<AccessRight>>();
            
        }

        public AccessGrid(string name, AccessGrid parent)
        {
            this.name = name;
            this.parent = parent;
            this.subgrids = new List<AccessGrid>();
            this.accessRights = new Dictionary<User, List<AccessRight>>();
        }

        public List<User> getUsersWithAccessRights()
        {
            var users = this.accessRights.Select(x => x.Key).ToList();
            if (parent != null)
            {
                var topLevel = parent.getUsersWithAccessRights();
                users = users.Union(topLevel).ToList();
            }
            return users.ToList();
        }

        public void addTag(string tag)
        {
            this.tags.Add(tag);
        }

        public List<AccessRight> getNormalizedUserRights(User user)
        {
            var localRights = accessRights.FirstOrDefault(x => x.Key == user).Value;
            if (localRights == null)
            {
                localRights = new List<AccessRight>();
            }

            if (parent != null)
            {
                var parentScopeRights = parent.getNormalizedUserRights(user);
                // simple variant, just aggregate for now, ignore the flags
                localRights.AddRange(parentScopeRights);
            }
         
            
            return localRights;
        }

        string getNormalizedName()
        {
            var name = "";
            if (parent != null)
            {
                name = parent.getNormalizedName();
                return name + "." + this.name;
            }
            return name;
        }

        public AccessGrid normalize()
        {
            Dictionary<User,List<AccessRight>> output = new Dictionary<User,List<AccessRight>>();
            foreach(var userPair in accessRights)
            {
                var normalizedRights = this.getNormalizedUserRights(userPair.Key);
                output[userPair.Key] = normalizedRights;
            }

            AccessGrid accessGrid = new AccessGrid(this.getNormalizedName(), null);
            accessGrid.accessRights = output;
            return accessGrid;
        }

        public void addSubgrid(AccessGrid s)
        {
            s.parent = this;
            subgrids.Add(s);
        }

        public void addAccessRightForUser(User u, AccessRight a)
        {
            var user = this.accessRights.First(x => x.Key == u);
            if (user.Value.Contains(a))
            {
                return;
            }

            user.Value.Add(a);
        }

        public void addUser(User u)
        {
            this.accessRights.Add(u, new List<AccessRight>());
        }

        public void newSubgrid(GridAddress path)
        {
            if (path.isLeaf())
            {
                var grid = new AccessGrid(path.top(), this);
                addSubgrid(grid);
            }
            else
            {
                var subgrid = this.subgrids.FirstOrDefault(x => path.topMatch(x.name));
                if (subgrid == null)
                {
                    throw new InvalidOperationException($"Cannot add to subgrid {name}, does not exist");
                }
                subgrid.newSubgrid(path.pop());
            }
        }

        public AccessGrid navigateTo(GridAddress path)
        {
            if (path.isLeaf())
            {
                return this;
            }
            else
            {
                var subgrid = this.subgrids.FirstOrDefault(x => path.pop().topMatch(x.name));
                if (subgrid == null)
                {
                    throw new InvalidOperationException($"Cannot add to subgrid {name}, does not exist");
                }
                return subgrid.navigateTo(path.pop());
            }
        }

        internal void removeAccessRight(User user, AccessRight rgt)
        {
            var rights = this.accessRights[user];
            rights.Remove(rgt);
        }
    }

    public class OrganisationalModel
    {

        public List<AccessGrid> allGrids = new List<AccessGrid> ();

        public OrganisationalModel()
        { }

        public void addGrid(string gridName)
        {
            // parentname is kindof considered a path. we check the
            // gridname against all existing grids for similarity.
            // e.g.
            // gridName = Arg.B
            // available Grids:
            // Arg
            // C
            // -> the new grid will be added as subgrid of Arg
            // gridname = A.B
            // -> the new grid should be added below a grid named A,
            // however that grid does not exist -> failure
            // 

            var adr = new GridAddress(gridName);
            
            if(adr.isLeaf())
            {
                allGrids.Add(new AccessGrid(adr.top(), null));
            }
            else
            {
                var grid = this.allGrids.FirstOrDefault(x => adr.topMatch(x.name));
                grid.newSubgrid(adr.pop());
            }
        }

        public AccessGrid getGrid(string address)
        {
            var adr = new GridAddress(address);
            var grid = this.allGrids.FirstOrDefault(x => adr.topMatch(x.name));
            
            if(adr.isLeaf())
            {
                return grid;
            }
            return grid.navigateTo(adr);
        }
    }
}
