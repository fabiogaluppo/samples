//Sample provided by Fabio Galuppo
//June 2014

//Partial implementation and Draft for Reactive Extensions (Rx) Workshop

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxWorkshop
{
    public enum Team
    {
        Brazil,
        Mexico,
        Cameroon,
        Croatia,
        Spain,
        Netherlands,
        Chile,
        Australia,
        Colombia,
        Greece,
        IvoryCoast,
        Japan,
        Uruguay,
        CostaRica,
        England,
        Italy,
        Swiss,
        Ecuador,
        France,
        Honduras,
        Argentina,
        BosniaHerzegovina,
        Iran,
        Nigeria,
        Germany,
        Portugal,
        Ghana,
        UnitedStates,
        Belgium,
        Algeria,
        Russia,
        SouthKorea
    }

    public sealed class Match
    {
        public Tuple<Team, Team> Teams { get; private set; }
        public DateTime Day { get; private set; }
        public Tuple<ushort, ushort> Score { get; private set; }

        private readonly Subject<Match> MatchSubject;

        public Match(Team home, Team visitor, DateTime matchDay)
        {
            if (home == visitor)
                throw new ArgumentException("home must be different from visitor");

            MatchSubject = new Subject<Match>();
            Teams = Tuple.Create(home, visitor);
            Day = matchDay;
            Score = null;
        }

        public Team Home { get { return Teams.Item1; } }
        public Team Visitor { get { return Teams.Item2; } }

        public bool HasScore { get { return Score != null; } }

        public ushort HomeScore { get { return Score.Item1; } }
        public ushort VisitorScore { get { return Score.Item2; } }

        public void FinalScore(ushort homeScore, ushort visitorScore)
        {
            if (HasScore)
                throw new InvalidOperationException("This match already has a score!");

            Score = Tuple.Create(homeScore, visitorScore);

            MatchSubject.OnNext(this);
            MatchSubject.OnCompleted();
        }

        public IObservable<Match> Result
        {
            get
            {
                return MatchSubject.AsObservable();
            }
        }
    }

    public sealed class WorldCup2014Table
    {
        //8 * 6 = 48 - Eliminatorias
        //8 - Oitavas de Final = Round of 16
        //4 - Quartas de Final = Quarter-finals
        //2 - Semifinal = Semi-finals
        //2 - Finais = Final
        public readonly Match[] Matches = new Match[48 + 8 + 4 + 2 + 2];

        private static DateTime NewDate(int day, int hour, int month = 6)
        {
            return new DateTime(2014, month, day, hour, 0, 0);
        }

        public WorldCup2014Table()
        {
            int i = 0;

            //Group.A
            Matches[i++] = new Match(Team.Brazil, Team.Croatia, NewDate(12, 17));
            Matches[i++] = new Match(Team.Mexico, Team.Cameroon, NewDate(13, 13));
            Matches[i++] = new Match(Team.Brazil, Team.Mexico, NewDate(17, 16));
            Matches[i++] = new Match(Team.Cameroon, Team.Croatia, NewDate(18, 19));
            Matches[i++] = new Match(Team.Croatia, Team.Mexico, NewDate(23, 17));
            Matches[i++] = new Match(Team.Cameroon, Team.Brazil, NewDate(23, 17));

            //Group.B
            Matches[i++] = new Match(Team.Spain, Team.Netherlands, NewDate(13, 16));
            Matches[i++] = new Match(Team.Chile, Team.Australia, NewDate(13, 19));
            Matches[i++] = new Match(Team.Australia, Team.Netherlands, NewDate(18, 13));
            Matches[i++] = new Match(Team.Spain, Team.Chile, NewDate(18, 16));
            Matches[i++] = new Match(Team.Australia, Team.Spain, NewDate(23, 13));
            Matches[i++] = new Match(Team.Netherlands, Team.Chile, NewDate(23, 13));

            //Group.C
            Matches[i++] = new Match(Team.Colombia, Team.Greece, NewDate(14, 13));
            Matches[i++] = new Match(Team.IvoryCoast, Team.Japan, NewDate(14, 22));
            Matches[i++] = new Match(Team.Colombia, Team.IvoryCoast, NewDate(19, 13));
            Matches[i++] = new Match(Team.Japan, Team.Greece, NewDate(19, 19));
            Matches[i++] = new Match(Team.Japan, Team.Colombia, NewDate(24, 17));
            Matches[i++] = new Match(Team.Greece, Team.IvoryCoast, NewDate(24, 17));

            //Group.D
            Matches[i++] = new Match(Team.Uruguay, Team.CostaRica, NewDate(14, 16));
            Matches[i++] = new Match(Team.England, Team.Italy, NewDate(14, 19));
            Matches[i++] = new Match(Team.Uruguay, Team.England, NewDate(19, 16));
            Matches[i++] = new Match(Team.Italy, Team.CostaRica, NewDate(20, 13));
            Matches[i++] = new Match(Team.CostaRica, Team.England, NewDate(24, 13));
            Matches[i++] = new Match(Team.Italy, Team.Uruguay, NewDate(24, 13));

            //Group.E
            Matches[i++] = new Match(Team.Swiss, Team.Ecuador, NewDate(15, 13));
            Matches[i++] = new Match(Team.France, Team.Honduras, NewDate(15, 16));
            Matches[i++] = new Match(Team.Swiss, Team.France, NewDate(20, 16));
            Matches[i++] = new Match(Team.Honduras, Team.Ecuador, NewDate(20, 19));
            Matches[i++] = new Match(Team.Honduras, Team.Swiss, NewDate(25, 17));
            Matches[i++] = new Match(Team.Ecuador, Team.France, NewDate(25, 17));

            //Group.F
            Matches[i++] = new Match(Team.Argentina, Team.BosniaHerzegovina, NewDate(15, 19));
            Matches[i++] = new Match(Team.Iran, Team.Nigeria, NewDate(16, 16));
            Matches[i++] = new Match(Team.Argentina, Team.Iran, NewDate(21, 13));
            Matches[i++] = new Match(Team.Nigeria, Team.BosniaHerzegovina, NewDate(21, 19));
            Matches[i++] = new Match(Team.Nigeria, Team.Argentina, NewDate(25, 13));
            Matches[i++] = new Match(Team.BosniaHerzegovina, Team.Iran, NewDate(25, 13));

            //Group.G
            Matches[i++] = new Match(Team.Germany, Team.Portugal, NewDate(16, 13));
            Matches[i++] = new Match(Team.Ghana, Team.UnitedStates, NewDate(16, 19));
            Matches[i++] = new Match(Team.Germany, Team.Ghana, NewDate(21, 16));
            Matches[i++] = new Match(Team.UnitedStates, Team.Portugal, NewDate(22, 19));
            Matches[i++] = new Match(Team.Portugal, Team.Ghana, NewDate(26, 13));
            Matches[i++] = new Match(Team.UnitedStates, Team.Germany, NewDate(26, 13));

            //Group.H
            Matches[i++] = new Match(Team.Belgium, Team.Algeria, NewDate(17, 13));
            Matches[i++] = new Match(Team.Russia, Team.SouthKorea, NewDate(17, 19));
            Matches[i++] = new Match(Team.Belgium, Team.Russia, NewDate(22, 13));
            Matches[i++] = new Match(Team.SouthKorea, Team.Algeria, NewDate(22, 16));
            Matches[i++] = new Match(Team.Algeria, Team.Russia, NewDate(26, 17));
            Matches[i++] = new Match(Team.SouthKorea, Team.Belgium, NewDate(26, 17));

            //Oitavas de Final
            //Matches[i++] = ...
        }

        public enum Group { A, B, C, D, E, F, G, H }

        private IEnumerable<Match> GroupRange(Group group)
        {
            for (int begin = 6 * (int)group, end = begin + 6, i = begin; i < end; ++i)
                yield return Matches[i];
        }

        public IEnumerable<Team> GetTeamsFromGroup(Group group)
        {
            return GetMatchesFromGroup(group).SelectMany(m => new Team[] { m.Home, m.Visitor }).Distinct();
        }

        public Group GetGroupFromTeam(Team team)
        {
            for (int i = (int)Group.A; i < (int)Group.H; ++i)
            {
                Group group = (Group)i;
                if (GetTeamsFromGroup(group).Contains(team))
                    return group;
            }

            return Group.H;
        }

        public Group GetGroupFromMatch(Match match)
        {
            for (int i = (int)Group.A; i < (int)Group.H; ++i)
            {
                Group group = (Group)i;
                if (GetMatchesFromGroup(group).Contains(match))
                    return group;
            }

            return Group.H;
        }

        public IEnumerable<Match> GetMatchesFromGroup(Group group)
        {
            return GroupRange(group);
        }

        public IEnumerable<Match> Groups
        {
            get
            {
                for (int i = (int)Group.A; i <= (int)Group.H; ++i)
                {
                    Group group = (Group)i;
                    foreach (var match in GetMatchesFromGroup(group))
                        yield return match;
                }
            }
        }
    }

    public sealed class PointInfo
    {
        public PointInfo(Team team)
        {
            Team = team;
        }

        public readonly Team Team;
        public ushort Points;
        public ushort GoalsFor;
        public ushort GoalsAgainst;
        public ushort Wins;
        public ushort Draws;
        public ushort Losses;

        public override string ToString()
        {
            var gd = Convert.ToInt16(GoalsFor) - Convert.ToInt16(GoalsAgainst);
            return String.Format("{0,-17}: pts:{1,2} w:{4} d:{5} l:{6} gf:{2,2} ga:{3,2} gd:{7,2}", Team, Points, GoalsFor, GoalsAgainst, Wins, Draws, Losses, gd);
        }
    }

    public sealed class GroupManager : IObserver<Match>
    {
        private WorldCup2014Table Table;

        private PointInfo[] Points;

        private Subject<Tuple<WorldCup2014Table.Group, Tuple<Team, Team>>> AdvancedToNextSubject;
        private Subject<Tuple<WorldCup2014Table.Group, Match>> MatchFromGroupSubject;

        public GroupManager(WorldCup2014Table table)
        {
            if (null == table)
                throw new ArgumentNullException("table");

            Table = table;

            Points = new PointInfo[48];
            for (int i = 0; i < 48; ++i)
                Points[i] = new PointInfo((Team)i);

            AdvancedToNextSubject = new Subject<Tuple<WorldCup2014Table.Group,Tuple<Team,Team>>>();
            
            MatchFromGroupSubject = new Subject<Tuple<WorldCup2014Table.Group, Match>>();
            
            foreach (var match in Table.Groups)
                match.Result.Subscribe(this);
        }

        public IObservable<Tuple<WorldCup2014Table.Group, Tuple<Team, Team>>> AdvancedToNext
        {
            get
            {
                return AdvancedToNextSubject.AsObservable();
            }
        }

        public IObservable<Tuple<WorldCup2014Table.Group, Match>> MatchFromGroup
        {
            get
            {
                return MatchFromGroupSubject.AsObservable();
            }
        }        

        void IObserver<Match>.OnCompleted()
        {            
        }
        
        public IEnumerable<PointInfo> GetPointsFromGroup(WorldCup2014Table.Group group)
        {
            return Points.Join(Table.GetTeamsFromGroup(group), x => x.Team, y => y, (x, y) => x);
        }

        public Tuple<Team, Team> GetFirstAndSecondFromGroup(WorldCup2014Table.Group group)
        {
            var temp = new Team[2];
            return GetPointsFromGroup(group)
                        .OrderByDescending(x => x.Points)
                        .ThenByDescending(x => x.GoalsFor - x.GoalsAgainst)
                        .ThenByDescending(x => x.Wins)
                        .Take(2)
                        .Aggregate
                        (
                            0,
                            (acc, x) => { temp[acc++] = x.Team; return acc; },
                            acc => Tuple.Create(temp[0], temp[1])
                        );
        }

        void IObserver<Match>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        private enum MatchResult
        {
            Draw,
            HomeWon,
            VisitorWon
        }

        void IObserver<Match>.OnNext(Match value)
        {
            MatchResult mr = MatchResult.Draw;
            if (value.HomeScore > value.VisitorScore) mr = MatchResult.HomeWon;
            else if (value.HomeScore < value.VisitorScore) mr = MatchResult.VisitorWon;
            
            var ps = Points.Where(x => x.Team == value.Home || x.Team == value.Visitor).ToArray();
            for (int i = 0; i < ps.Length; ++i)
            {
                var p = ps[i];

                if (p.Team == value.Home)
                {
                    p.GoalsFor += value.HomeScore;
                    p.GoalsAgainst += value.VisitorScore;
                    if (mr == MatchResult.HomeWon)
                    {
                        p.Points += 3;
                        p.Wins++;
                        continue;
                    } 
                    else if (mr == MatchResult.VisitorWon)
                    {
                        p.Losses++;
                        continue;
                    }
                }
                else //if (p.Team == value.Visitor)
                {
                    p.GoalsFor += value.VisitorScore;
                    p.GoalsAgainst += value.HomeScore;
                    if (mr == MatchResult.VisitorWon)
                    {
                        p.Points += 3;
                        p.Wins++;
                        continue;
                    }
                    else if (mr == MatchResult.HomeWon)
                    {
                        p.Losses++;
                        continue;
                    }
                }

                p.Points++; //if (mr == MatchResult.Draw)
                p.Draws++;
            }

            //
            Publish(value);
        }

        private void Publish(Match match)
        {
            var group = Table.GetGroupFromMatch(match);

            MatchFromGroupSubject.OnNext(Tuple.Create(group, match));
            
            var count = GetPointsFromGroup(group)
                            .Where(p => p.Wins + p.Losses + p.Draws == 3)
                            .Count();
            if (count == 4)
            {
                AdvancedToNextSubject.OnNext(Tuple.Create(group, GetFirstAndSecondFromGroup(group)));
            }
        }
    }

    class Program
    {
        static void RoundOf16Handler(Tuple<WorldCup2014Table.Group, Tuple<Team, Team>> x)
        {
            Console.WriteLine("From group {0}, {1} takes 1st and {2} takes 2nd", x.Item1, x.Item2.Item1, x.Item2.Item2);
            Console.WriteLine();
        }

        //static void DisplayScore(IList<Tuple<WorldCup2014Table.Group, Match>> xs)
        static void DisplayScore(Tuple<GroupManager, WorldCup2014Table.Group> x)
        {
            var gm = x.Item1;
            var group = x.Item2;
            Console.WriteLine("Group {0}:", group);
            foreach (var y in gm.GetPointsFromGroup(group))
                Console.WriteLine(y);
            Console.WriteLine(new string('-', 56));
        }

        static void Main(string[] args)
        {
            WorldCup2014Table w = new WorldCup2014Table();
            GroupManager gm = new GroupManager(w);

            var roundOf16Sub = gm.AdvancedToNext
                                 .Subscribe(RoundOf16Handler);

            var scoreSub = gm.MatchFromGroup
                             .Buffer(2)
                             .Select(x => Tuple.Create(gm, x.First().Item1))
                             .Subscribe(DisplayScore);

            //w.Matches[0].FinalScore(3, 1);
            //w.Matches[1].FinalScore(1, 0);
            //w.Matches[2].FinalScore(0, 0);
            //w.Matches[3].FinalScore(0, 4);
            //w.Matches[4].FinalScore(1, 3);
            //w.Matches[5].FinalScore(1, 4);

            //w.Matches[6].FinalScore(1, 5);
            //w.Matches[7].FinalScore(3, 1);
            //w.Matches[8].FinalScore(2, 3);
            //w.Matches[9].FinalScore(0, 2);
            //w.Matches[10].FinalScore(0, 3);
            //w.Matches[11].FinalScore(2, 0);

            //imagine this as a live and timed stream
            w.Matches[0].FinalScore(3, 1);
            w.Matches[1].FinalScore(1, 0);
            w.Matches[6].FinalScore(1, 5);
            w.Matches[7].FinalScore(3, 1);
            w.Matches[2].FinalScore(0, 0);
            w.Matches[3].FinalScore(0, 4);
            w.Matches[8].FinalScore(2, 3);
            w.Matches[9].FinalScore(0, 2);
            w.Matches[10].FinalScore(0, 3);
            w.Matches[11].FinalScore(2, 0);
            w.Matches[4].FinalScore(1, 3);
            w.Matches[5].FinalScore(1, 4);
            //...

            scoreSub.Dispose();
            roundOf16Sub.Dispose();
            
            Console.ReadLine();
        }
    }
}
