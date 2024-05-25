using System;
using System.Collections.Generic;

namespace TaskManager
{
    public class Manager : IManager
    {
        private readonly IDictionary<string, Task> byId;
        private readonly IDictionary<string, Dictionary<string, Task>> dependenciesById;
        private readonly IDictionary<string, Dictionary<string, Task>> depedendantById;

        public Manager()
        {
            this.byId = new Dictionary<string, Task>();
            this.dependenciesById = new Dictionary<string, Dictionary<string, Task>>();
            this.depedendantById = new Dictionary<string, Dictionary<string, Task>>();
        }

        public void AddTask(Task task)
        {
            if (this.Contains(task.Id))
            {
                throw new ArgumentException();
            }
            
            this.byId.Add(task.Id, task);
            this.dependenciesById.Add(task.Id, new Dictionary<string, Task>());
            this.depedendantById.Add(task.Id, new Dictionary<string, Task>());
        }

        public void RemoveTask(string taskId)
        {
            if (!this.Contains(taskId))
            {
                throw new ArgumentException();
            }

            var dependencies = this.GetDependencies(taskId);
            foreach (var task in dependencies)
            {
                this.depedendantById[task.Id].Remove(taskId);
            }

            var dependantTasks = this.GetDependents(taskId);
            foreach (var task in dependantTasks)
            {
                this.dependenciesById[task.Id].Remove(taskId);
            }
            
            this.byId.Remove(taskId);
            this.dependenciesById.Remove(taskId);
        }

        public bool Contains(string taskId)
        {
            return this.byId.ContainsKey(taskId);
        }

        public Task Get(string taskId)
        {
            if (!this.Contains(taskId))
            {
                throw new ArgumentException();
            }

            return this.byId[taskId];
        }

        public int Size()
        {
            return this.byId.Count;
        }

        public void AddDependency(string taskId, string dependentTaskId)
        {
            var task = this.Get(taskId);
            var dependency = this.Get(dependentTaskId);
            
            var dependencies = this.GetDependencies(dependentTaskId);
            foreach (var d in dependencies)
            {
                if (d.Id == taskId)
                {
                    throw new ArgumentException();
                }
                
                this.dependenciesById[taskId].Add(d.Id, d);
                this.depedendantById[d.Id].Add(taskId, task);
            }

            var dependants = this.GetDependents(taskId);
            foreach (var d in dependants)
            {
                this.dependenciesById[d.Id].Add(dependency.Id, dependency);
                this.depedendantById[dependency.Id].Add(d.Id, d);
            }

            this.dependenciesById[taskId].Add(dependentTaskId, dependency);
            this.depedendantById[dependentTaskId].Add(taskId, task);
        }

        public void RemoveDependency(string taskId, string dependentTaskId)
        {
            if (!this.Contains(taskId))
            {
                throw new ArgumentException();
            }

            if (!this.Contains(dependentTaskId))
            {
                throw new ArgumentException();
            }

            if (!this.dependenciesById[taskId].ContainsKey(dependentTaskId))
            {
                throw new ArgumentException();
            }

            var dependants = this.GetDependents(taskId);
            foreach (var dependant in dependants)
            {
                this.dependenciesById[dependant.Id].Remove(dependentTaskId);
                this.depedendantById[dependentTaskId].Remove(dependant.Id);
            }

            this.dependenciesById[taskId].Remove(dependentTaskId);
            this.depedendantById[dependentTaskId].Remove(taskId);
        }

        public IEnumerable<Task> GetDependencies(string taskId)
        {
            if (!this.Contains(taskId))
            {
                return new List<Task>();
            }
            
            return this.dependenciesById[taskId].Values;
        }

        public IEnumerable<Task> GetDependents(string taskId)
        {
            if (!this.Contains(taskId))
            {
                return new List<Task>();
            }
            
            return this.depedendantById[taskId].Values;
        }
    }
}