# Project Vision Statement

## Vision

Build a working **ASP.NET Core MVC** web application that enables users to generate intelligent research and recommendation reports from either a single research topic or multiple topics to compare, configure report preferences, and export the results in multiple formats.

The application should demonstrate:

* Strong ASP.NET Core MVC architecture
* Practical AI integration
* Clean separation of concerns
* Extensible design that supports multiple AI providers
* Production-quality code suitable for a professional portfolio

The application will use free-tier AI providers such as **Groq** or **Gemini** and help users:

* Understand unfamiliar topics clearly
* Research one topic deeply with explanations, visuals, and practical guidance
* Compare multiple options intelligently
* Explain relationships and tradeoffs
* Recommend the best approach, next steps, or best choice for different scenarios

---

# Product Direction

The application is intended for **multiple types of users**, rather than a single target audience.

Users should be able to:

* Define what they want to research
* Choose whether they want a single-topic research report or a comparison report
* Choose the report style
* Select the desired technical depth
* Specify evaluation criteria when useful
* Preview the generated report
* Download the report in multiple formats

The first release will focus on delivering a **fully functional application**, not merely a product specification.

### Included in Version 1

* User registration and authentication
* User-owned report history
* AI-powered report generation
* Report preview
* Multi-format export

### Excluded from Version 1

* Admin dashboard
* Self-service template management

Any report templates or configuration can be managed manually by the project owner.

---

# Core Report Inputs

Each report request should collect the following information:

## Required Inputs

* Report mode
* One or more topics
* Target audience
* Report style
* Technical depth level
* Output format

## Conditionally Required Inputs

* Preferred evaluation criteria for comparison reports
* Research focus areas for single-topic reports, when the user wants to guide the explanation

## Optional Inputs

* Industry or business domain
* Current technology stack
* Performance requirements
* Security requirements
* Budget considerations
* Must-include notes
* Must-avoid notes

The application may recommend a suitable report style based on the selected category, while still allowing the user to override the suggestion.

---

# Report Generation Requirements

Generated reports should prioritize **clear explanations** over excessive technical detail.

The content should remain understandable even for readers who are unfamiliar with the selected topics.

Technical reports may include:

* Small illustrative code snippets
* Architecture examples
* Configuration samples

Large code listings should be avoided.

---

# Standard Report Structure

Generated reports should adapt their structure to the selected report mode.

## Single-Topic Research Report Structure

A single-topic report should generally include:

1. Executive Summary
2. Topic Overview
3. Why the Topic Matters
4. Key Concepts and Terminology
5. Visual Explanation or Diagram
6. Practical Use Cases
7. Benefits, Risks, and Tradeoffs
8. Recommended Approach or Best Practices
9. Implementation Notes or Learning Path
10. References or Cited Sources (when live research is used)

## Comparison Report Structure

A comparison report should generally include:

1. Executive Summary
2. Introduction
3. Explanation of Each Topic
4. Relationships Between Topics
5. Comparison Table
6. Decision Matrix
7. Scenario-Based Recommendations
8. Risks and Tradeoffs
9. Implementation Notes
10. References or Cited Sources (when live research is used)

---

# Technology Stack

## Backend

* ASP.NET Core MVC
* SQL Server

## Authentication

* Standard user authentication

## Artificial Intelligence

* Groq (Free Tier)
* Google Gemini (Free Tier)

The architecture should abstract AI providers so they can be replaced or extended without changing the application's core business logic.

Report generation may combine:

* AI-generated knowledge
* Internal templates
* Structured prompting
* Visual explanation generation, such as diagrams or chart-ready sections
* Live research with citations (when appropriate)

---

# Supported Export Formats

The initial release should support exporting reports as:

* Markdown (.md)
* PDF (.pdf)
* Microsoft Word (.docx)
* HTML (.html)

---

# Measurable Project Goals

The project will be considered successful when users can:

* Register and log in securely.
* Manage their own generated reports.
* Create report requests by specifying:

  * Report mode
  * Topic or topics
  * Audience
  * Report style
  * Technical depth
  * Research focus areas or comparison criteria
  * Output preferences
* Generate structured research and recommendation reports using a real AI provider.
* Receive reports that clearly explain unfamiliar topics, either as focused single-topic research or as comparisons using engineering and business criteria.
* Preview generated reports within the application.
* Download reports in Markdown, PDF, DOCX, and HTML formats.
* Reopen previously generated reports stored in SQL Server.

From an engineering perspective, the codebase should demonstrate:

* Clean ASP.NET Core MVC architecture
* Separate services for:

  * AI generation
  * Report orchestration
  * Data persistence
  * Export functionality
* Swappable AI provider implementation
* Avoidance of paid-only dependencies
* Production-quality code suitable for showcasing architecture and AI integration in a professional portfolio.
