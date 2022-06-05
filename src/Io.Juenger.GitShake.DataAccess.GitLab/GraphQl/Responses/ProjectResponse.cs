using System;
using Newtonsoft.Json;

namespace Scrummy.DataAccess.GitLab.GraphQl.Responses
{
    internal class ProjectResponse
    {
        [JsonProperty("project")]
        public Project Project { get; set; }
    }

    internal class Project
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("board")]
        public Board Board { get; set; }
        
        [JsonProperty("boards")]
        public Boards Boards { get; set; }
    }

    internal class Boards
    {
        [JsonProperty("edges")]
        public BoardsEdge[] BoardsEdges { get; set; }
    }

    internal class BoardsEdge
    {
        [JsonProperty("node")]
        public Board Board { get; set; }
    }
    
    internal class Board
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("lists")]
        public Lists Lists { get; set; }
    }

    internal class Lists
    {
        [JsonProperty("edges")]
        public ListsEdge[] ListsEdges { get; set; }
    }

    internal class ListsEdge
    {
        [JsonProperty("node")]
        public List List { get; set; }
    }

    internal class List
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("issues")]
        public Issues Issues { get; set; }
    }

    internal class Issues
    {
        [JsonProperty("edges")]
        public IssuesEdge[] IssuesEdges { get; set; }
    }

    internal class IssuesEdge
    {
        [JsonProperty("node")]
        public Issue Issue { get; set; }
    }

    internal class Issue
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("webUrl")]
        public Uri WebUrl { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("closedAt")]
        public DateTimeOffset? ClosedAt { get; set; }
        
        [JsonProperty("labels")]
        public Labels Labels { get; set; }
    }

    internal class Labels
    {
        [JsonProperty("edges")]
        public LabelsEdge[] LabelsEdges { get; set; }
    }

    internal class LabelsEdge
    {
        [JsonProperty("node")]
        public Label Label { get; set; }
    }

    internal class Label
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    internal enum State { Closed, Opened };
}