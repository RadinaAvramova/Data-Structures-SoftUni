using System;
using System.Collections.Generic;
using System.Linq;

namespace Exam.MovieDatabase
{
    public class MovieDatabase : IMovieDatabase
    {
        private HashSet<Actor> actors = new HashSet<Actor>();

        private HashSet<Actor> newbieActors = new HashSet<Actor>();

        private HashSet<Movie> movies = new HashSet<Movie>();

        private Dictionary<string, HashSet<Movie>> actorsWithMovies = new Dictionary<string, HashSet<Movie>>();

        public void AddActor(Actor actor)
        {
            this.actors.Add(actor);
            this.newbieActors.Add(actor);
            this.actorsWithMovies.Add(actor.Id, new HashSet<Movie>());
        }

        public void AddMovie(Actor actor, Movie movie)
        {
            if (!this.actorsWithMovies.ContainsKey(actor.Id))
            {
                throw new ArgumentException();
            }

            this.actorsWithMovies[actor.Id].Add(movie);
            this.movies.Add(movie);
            this.newbieActors.Remove(actor);
        }

        public bool Contains(Actor actor)
        {
            return this.actors.Contains(actor);
        }

        public bool Contains(Movie movie)
        {
            return this.movies.Contains(movie);
        }

        public IEnumerable<Actor> GetActorsOrderedByMaxMovieBudgetThenByMoviesCount()
        {
            return this.actors
                .OrderByDescending(a => this.actorsWithMovies[a.Id].Max(m => m.Budget))
                .ThenByDescending(a => this.actorsWithMovies[a.Id].Count);
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return this.movies;
        }

        public IEnumerable<Movie> GetMoviesInRangeOfBudget(double lower, double upper)
        {
            return this.movies
                .Where(m => m.Budget >= lower && m.Budget <= upper)
                .OrderByDescending(m => m.Rating);
        }

        public IEnumerable<Movie> GetMoviesOrderedByBudgetThenByRating()
        {
            return this.movies
                .OrderByDescending(m => m.Budget)
                .ThenByDescending(m => m.Rating);
        }

        public IEnumerable<Actor> GetNewbieActors()
        {
            return this.newbieActors;
        }
    }
}
