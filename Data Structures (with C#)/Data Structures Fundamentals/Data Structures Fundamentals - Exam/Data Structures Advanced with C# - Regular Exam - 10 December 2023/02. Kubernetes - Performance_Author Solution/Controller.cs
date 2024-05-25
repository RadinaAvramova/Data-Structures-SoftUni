using System;
using System.Collections.Generic;
using System.Linq;

namespace Kubernetes
{
    public class Controller : IController
    {
        private readonly IDictionary<string, Pod> byId;
        private readonly IDictionary<string, Dictionary<string, Pod>> byNamespace;

        public Controller()
        {
            this.byId = new Dictionary<string, Pod>();
            this.byNamespace = new Dictionary<string, Dictionary<string, Pod>>();
        }
        
        public void Deploy(Pod pod)
        {
            this.byId.Add(pod.Id, pod);

            if (!this.byNamespace.ContainsKey(pod.Namespace))
            {
                this.byNamespace.Add(pod.Namespace, new Dictionary<string, Pod>());
            }
            
            this.byNamespace[pod.Namespace].Add(pod.Id, pod);
        }

        public bool Contains(string podId)
        {
            return this.byId.ContainsKey(podId);
        }

        public int Size()
        {
            return this.byId.Count;
        }

        public Pod GetPod(string podId)
        {
            if (!this.byId.ContainsKey(podId))
            {
                throw new ArgumentException();
            }

            return this.byId[podId];
        }

        public void Uninstall(string podId)
        {
            if (!this.byId.ContainsKey(podId))
            {
                throw new ArgumentException();
            }

            var pod = this.byId[podId];

            this.byId.Remove(podId);
            this.byNamespace[pod.Namespace].Remove(podId);
        }

        public void Upgrade(Pod pod)
        {
            if (!this.byId.ContainsKey(pod.Id))
            {
                this.Deploy(pod);
                return;
            }

            var oldPod = this.GetPod(pod.Id);
            if (pod.Namespace == oldPod.Namespace)
            {
                // If the namespace matches
                // Then we have to update only the other properties
                this.byNamespace[pod.Namespace][pod.Id] = pod;
                this.byId[pod.Id] = pod;
                return;
            }

            // If the pod points to another namespace
            // Then we need to remove the pod from the old namespace
            // And add it to the new one
            this.byNamespace[oldPod.Namespace].Remove(oldPod.Id);

            if (!this.byNamespace.ContainsKey(pod.Namespace))
            {
                this.byNamespace.Add(pod.Namespace, new Dictionary<string, Pod>());
            }
            
            this.byNamespace[pod.Namespace].Add(pod.Id, pod);
            this.byId[pod.Id] = pod;
        }

        public IEnumerable<Pod> GetPodsInNamespace(string @namespace)
        {
            return this.byNamespace[@namespace].Values;
        }

        public IEnumerable<Pod> GetPodsBetweenPort(int lowerBound, int upperBound)
        {
            return this.byId.Values
                .Where(p => p.Port >= lowerBound && p.Port <= upperBound);
        }

        public IEnumerable<Pod> GetPodsOrderedByPortThenByName()
        {
            return this.byId.Values
                .OrderByDescending(p => p.Port)
                .ThenBy(p => p.Namespace);
        }
    }
}