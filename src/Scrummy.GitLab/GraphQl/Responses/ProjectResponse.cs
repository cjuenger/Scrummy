using System;
using Newtonsoft.Json;

namespace Scrummy.GitLab.GraphQl.Responses
{
    public class ProjectResponse
    {
        [JsonProperty("project")]
        public Project Project { get; set; }
    }

    public class Project
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

    public class Boards
    {
        [JsonProperty("edges")]
        public BoardsEdge[] BoardsEdges { get; set; }
    }

    public class BoardsEdge
    {
        [JsonProperty("node")]
        public Board Board { get; set; }
    }
    
    public class Board
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("lists")]
        public Lists Lists { get; set; }
    }

    public class Lists
    {
        [JsonProperty("edges")]
        public ListsEdge[] ListsEdges { get; set; }
    }

    public class ListsEdge
    {
        [JsonProperty("node")]
        public List List { get; set; }
    }

    public class List
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("issues")]
        public Issues Issues { get; set; }
    }

    public class Issues
    {
        [JsonProperty("edges")]
        public IssuesEdge[] IssuesEdges { get; set; }
    }

    public class IssuesEdge
    {
        [JsonProperty("node")]
        public Issue Issue { get; set; }
    }

    public class Issue
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

    public class Labels
    {
        [JsonProperty("edges")]
        public LabelsEdge[] LabelsEdges { get; set; }
    }

    public class LabelsEdge
    {
        [JsonProperty("node")]
        public Label Label { get; set; }
    }

    public class Label
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public enum State { Closed, Opened };
}