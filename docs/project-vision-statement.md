# Project Vision Statement

## Vision

Build a working **ASP.NET Core MVC** web application that enables users to generate intelligent research and recommendation reports by selecting topics to compare, configuring report preferences, and exporting the results in multiple formats.

The application should demonstrate:

* Strong ASP.NET Core MVC architecture
* Practical AI integration
* Clean separation of concerns
* Extensible design that supports multiple AI providers
* Production-quality code suitable for a professional portfolio

The application will use free-tier AI providers such as **Groq** or **Gemini** and help users:

* Understand unfamiliar topics clearly
* Compare multiple options intelligently
* Explain relationships and tradeoffs
* Recommend the best choice for different scenarios

---

# Product Direction

The application is intended for **multiple types of users**, rather than a single target audience.

Users should be able to:

* Define what they want to research
* Choose the report style
* Select the desired technical depth
* Specify comparison criteria
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

* Topics to compare
* Target audience
* Report style
* Technical depth level
* Preferred comparison criteria
* Output format

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

Every generated report should generally include:

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

  * Topics
  * Audience
  * Report style
  * Technical depth
  * Comparison criteria
  * Output preferences
* Generate structured recommendation reports using a real AI provider.
* Receive reports that clearly explain unfamiliar topics and compare them using engineering and business criteria.
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
