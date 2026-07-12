# Task Breakdown and Delegation Plan

## Project Context

The project is a working ASP.NET Core MVC application that helps users generate intelligent research and recommendation reports. Users can choose topics to compare, set report preferences, preview the generated report, and download it in Markdown, PDF, DOCX, and HTML formats.

The project is intended to demonstrate strong .NET architecture and practical AI integration, not just produce a single static report.

## Delegation Principle

The best delegation model for this project is not "human decides everything, AI writes everything." A stronger model is:

- The human owns product judgment, scope control, acceptance standards, and final decisions.
- AI supports research, comparison, drafting, implementation acceleration, test ideas, and documentation.
- Human and AI collaborate most closely where ambiguity is high: requirements, architecture tradeoffs, prompt design, report quality, and deciding what "good enough" means for a portfolio-grade release.

This matters because the project is supposed to demonstrate architectural thinking. If AI simply generates code without human direction, the result may work but fail to show clear design judgment.

## Major Tasks and Delegation Decisions

### 1. Product Scope and Requirements

**Task**

Define what the application should do in version 1, including user flows, inputs, outputs, report structure, and boundaries.

**Skills, Knowledge, or AI Capabilities Needed**

- Product thinking
- Requirements analysis
- Ability to distinguish core features from optional features
- Familiarity with report-generation workflows
- Ability to translate a broad idea into buildable user stories

**Uniquely Human Strengths**

- Deciding what is personally valuable for the portfolio
- Choosing the right level of ambition
- Recognizing which features show the strongest architecture
- Saying no to features that make the project larger without improving the learning goal

**AI Capabilities That Help**

- Turning rough ideas into structured requirements
- Suggesting missing workflows or edge cases
- Comparing possible feature sets
- Drafting acceptance criteria

**Collaboration Impact**

Collaboration is especially valuable here because the human may have a strong vision but not yet know how to shape it into a practical first version. AI can challenge scope and suggest structure, while the human keeps the project aligned with personal goals.

**Delegation Decision**

Human-led, AI-assisted. The human makes final scope decisions. AI helps organize, question, and refine the requirements.

---

### 2. Architecture and Technical Design

**Task**

Design the ASP.NET Core MVC architecture, including controllers, services, data models, AI provider abstraction, export services, authentication, and SQL Server persistence.

**Skills, Knowledge, or AI Capabilities Needed**

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- Authentication and authorization
- Dependency injection
- Clean separation of concerns
- Service-oriented application design
- Provider abstraction patterns

**Uniquely Human Strengths**

- Choosing architecture that demonstrates skill without becoming over-engineered
- Making tradeoffs between clean design and speed
- Deciding which abstractions are useful now versus unnecessary complexity
- Ensuring the architecture is explainable in a portfolio or interview

**AI Capabilities That Help**

- Proposing folder structures
- Drafting service interfaces
- Identifying missing layers
- Explaining tradeoffs between patterns
- Reviewing architecture for coupling or responsibility problems

**Collaboration Impact**

This is one of the highest-value collaboration areas. The human should challenge AI-generated architecture and ask: "Does this prove the skills I want to demonstrate?" AI can provide options, but the human should select the design.

**Delegation Decision**

Shared collaboration. AI can draft the architecture, but the human should review and approve the final structure.

---

### 3. Database and Domain Modeling

**Task**

Design the SQL Server schema and domain models for users, report requests, generated reports, report formats, AI provider metadata, and saved report history.

**Skills, Knowledge, or AI Capabilities Needed**

- Relational database design
- Entity Framework Core modeling
- ASP.NET Identity integration
- Data normalization
- Migration management
- Understanding report lifecycle states

**Uniquely Human Strengths**

- Deciding which data is important for the product experience
- Choosing what should be persisted versus generated on demand
- Keeping the schema understandable and portfolio-friendly

**AI Capabilities That Help**

- Suggesting entity models
- Drafting EF Core configurations
- Identifying relationships and constraints
- Creating seed data ideas
- Reviewing schema for missing fields

**Collaboration Impact**

AI can quickly produce a solid first schema, but human review matters because poor data modeling can make later features awkward. The collaboration should focus on asking whether the schema supports real workflows.

**Delegation Decision**

AI-drafted, human-reviewed. AI can generate the first model set, while the human validates it against the user flow.

---

### 4. AI Provider Integration

**Task**

Integrate free-tier AI providers such as Groq and Gemini, while keeping the application flexible enough to swap providers later.

**Skills, Knowledge, or AI Capabilities Needed**

- HTTP API integration
- Secure configuration management
- Provider abstraction
- Prompt engineering
- Error handling and retries
- Rate-limit awareness
- JSON parsing and response validation

**Uniquely Human Strengths**

- Choosing acceptable provider behavior
- Deciding how much failure handling is enough for version 1
- Judging whether AI output is actually useful
- Protecting the project from becoming dependent on one provider

**AI Capabilities That Help**

- Drafting integration code
- Designing provider interfaces
- Creating prompts
- Suggesting fallback strategies
- Explaining provider differences

**Collaboration Impact**

This is a strong AI collaboration area because AI can help design prompts and provider contracts. However, the human should inspect outputs carefully, because AI-generated text can sound confident while being vague or inaccurate.

**Delegation Decision**

Shared collaboration. AI helps implement and refine provider integration, while the human evaluates output quality and provider fit.

---

### 5. Report Generation Pipeline

**Task**

Build the workflow that turns user inputs into a structured report with sections such as executive summary, topic explanations, comparison table, decision matrix, recommendations, risks, implementation notes, and references.

**Skills, Knowledge, or AI Capabilities Needed**

- Prompt design
- Structured output design
- Markdown generation
- Content quality evaluation
- Technical writing
- Report template design
- Scenario-based recommendation logic

**Uniquely Human Strengths**

- Defining what "smart comparison" means
- Judging clarity for a reader who does not know the topic
- Deciding when the report is too shallow, too verbose, or too technical
- Ensuring recommendations are practical, not generic

**AI Capabilities That Help**

- Drafting report sections
- Explaining unfamiliar technologies
- Generating comparison tables
- Suggesting decision criteria
- Rewriting content for different audiences

**Collaboration Impact**

This may be the most important collaboration point in the project. The system's value depends on report quality. AI should generate the content, but the human needs to define standards and test outputs with real examples.

**Delegation Decision**

AI-executed, human-directed and human-evaluated. AI writes the report content, but the human defines the structure and quality bar.

---

### 6. Live Research and Citations

**Task**

Support cited research when current or source-backed information is needed.

**Skills, Knowledge, or AI Capabilities Needed**

- Source evaluation
- Citation formatting
- Research summarization
- Awareness of outdated information risk
- Ability to distinguish official documentation from weaker sources

**Uniquely Human Strengths**

- Deciding which sources are trustworthy
- Knowing when citations are required
- Evaluating whether evidence supports the recommendation
- Avoiding blind trust in AI summaries

**AI Capabilities That Help**

- Summarizing source material
- Extracting key comparisons
- Organizing citations
- Highlighting conflicts between sources

**Collaboration Impact**

Collaboration matters because AI can process sources quickly, but the human should apply discernment. The report should not only cite sources; it should use them responsibly.

**Delegation Decision**

Human-governed, AI-assisted. AI can gather and summarize, but humans should validate important claims and source quality.

---

### 7. Export System

**Task**

Generate downloadable reports in Markdown, PDF, DOCX, and HTML.

**Skills, Knowledge, or AI Capabilities Needed**

- Markdown rendering
- HTML generation
- PDF conversion
- DOCX generation
- File streaming in ASP.NET Core MVC
- Formatting consistency

**Uniquely Human Strengths**

- Defining acceptable document polish
- Checking whether exported reports look professional
- Deciding which layout details matter for version 1

**AI Capabilities That Help**

- Recommending libraries
- Drafting export service code
- Creating formatting templates
- Testing conversion edge cases

**Collaboration Impact**

AI can accelerate implementation, but visual verification still needs human judgment. A report that exports successfully but looks unprofessional would weaken the project.

**Delegation Decision**

AI-assisted implementation, human QA. AI writes much of the export logic; the human reviews generated files.

---

### 8. Authentication and User Report History

**Task**

Allow users to register, log in, generate reports, and view their own saved report history.

**Skills, Knowledge, or AI Capabilities Needed**

- ASP.NET Core Identity
- Authorization
- User-owned data access
- Secure data filtering
- MVC forms and validation

**Uniquely Human Strengths**

- Deciding the simplest secure user experience
- Avoiding unnecessary admin features
- Ensuring the feature supports the demonstration goal

**AI Capabilities That Help**

- Scaffolding Identity-related code
- Suggesting controller actions and views
- Reviewing authorization checks
- Creating validation logic

**Collaboration Impact**

The main collaboration value is in security review. AI can generate boilerplate quickly, but the human should verify that users cannot access each other's reports.

**Delegation Decision**

AI-assisted implementation with careful human review.

---

### 9. MVC User Interface

**Task**

Build the main screens: dashboard, create report form, report preview, report history, and download actions.

**Skills, Knowledge, or AI Capabilities Needed**

- Razor views
- Bootstrap or CSS styling
- Form design
- Validation messages
- Usability thinking
- Responsive layout

**Uniquely Human Strengths**

- Deciding what feels clear and not boring
- Balancing enough options with a simple workflow
- Reviewing whether the UI communicates the value of the app

**AI Capabilities That Help**

- Drafting Razor views
- Suggesting form organization
- Writing validation messages
- Improving labels and helper text

**Collaboration Impact**

Collaboration is useful because the user experience must collect many inputs without making the user tired. AI can suggest layout patterns, but the human should decide whether the flow feels natural.

**Delegation Decision**

Shared. AI can build the first version, while the human gives feedback on clarity and flow.

---

### 10. Testing and Quality Review

**Task**

Verify that the app works correctly, reports are generated, exports are downloadable, authentication protects user data, and AI failure cases are handled.

**Skills, Knowledge, or AI Capabilities Needed**

- Unit testing
- Integration testing
- Manual testing
- Security review
- Test case design
- Failure mode analysis

**Uniquely Human Strengths**

- Deciding whether the product feels complete
- Catching confusing workflows
- Reviewing real report quality
- Prioritizing bugs that affect the portfolio demonstration

**AI Capabilities That Help**

- Generating test cases
- Suggesting edge cases
- Reviewing code for missing validation
- Creating sample report prompts
- Explaining failures

**Collaboration Impact**

This is a strong place for AI as a second reviewer. AI can help find cases the human forgot, but the human decides which issues are important enough to fix before calling version 1 complete.

**Delegation Decision**

Shared, with human final acceptance.

---

### 11. Documentation and Portfolio Presentation

**Task**

Document the project so it can be understood by instructors, reviewers, or interviewers.

**Skills, Knowledge, or AI Capabilities Needed**

- Technical writing
- Architecture explanation
- Setup documentation
- Feature summary writing
- Portfolio storytelling

**Uniquely Human Strengths**

- Explaining why the project matters personally
- Highlighting learning goals
- Choosing which architectural choices to emphasize
- Connecting the project to career goals

**AI Capabilities That Help**

- Drafting README sections
- Writing architecture summaries
- Creating setup instructions
- Polishing language
- Generating diagrams from described architecture

**Collaboration Impact**

AI can help make the project sound professional, but the human should keep the story authentic. The portfolio should show real judgment, not just generated polish.

**Delegation Decision**

Human-led, AI-polished.

## Consolidated Project Plan

### Phase 1: Foundation

1. Confirm final version 1 scope.
2. Create ASP.NET Core MVC solution.
3. Configure SQL Server and Entity Framework Core.
4. Add authentication for normal users.
5. Define domain models for report requests, generated reports, report formats, and user history.

**Delegation Choice**

AI can scaffold and suggest structure. Human should approve scope, architecture, and model design.

### Phase 2: Core Report Workflow

1. Build create-report form.
2. Capture topics, target audience, style, depth, criteria, output preferences, and optional constraints.
3. Save report request data.
4. Build report preview page.
5. Build report history page.

**Delegation Choice**

AI can draft controllers, views, and view models. Human should review the workflow for simplicity and clarity.

### Phase 3: AI Generation

1. Create AI provider abstraction.
2. Implement at least one free-tier provider first.
3. Add a second provider if time allows.
4. Create structured prompt templates.
5. Generate Markdown as the canonical report content.
6. Store generated reports in SQL Server.

**Delegation Choice**

AI can help design prompts and provider code. Human should evaluate report quality and decide whether outputs meet the standard.

### Phase 4: Export and Preview

1. Render Markdown preview in the app.
2. Export Markdown directly.
3. Convert generated content to HTML.
4. Generate PDF.
5. Generate DOCX.
6. Verify formatting and download behavior.

**Delegation Choice**

AI can recommend libraries and write export services. Human should inspect exported documents for professionalism.

### Phase 5: Quality, Testing, and Polish

1. Test authentication and user-owned report access.
2. Test report generation with several topic sets.
3. Test AI failure and empty-response cases.
4. Test all export formats.
5. Improve UI labels, validation, and error messages.
6. Document setup and architecture.

**Delegation Choice**

AI can generate test cases and review code. Human should decide final readiness.

## Key Collaboration Questions to Revisit Later

- Is the app trying to demonstrate too many things at once?
- Does the architecture feel clean, or does it contain abstractions only added to look advanced?
- Are generated reports genuinely useful, or only nicely formatted?
- Can a new user understand what to input without getting bored?
- Do the exports look professional enough for a portfolio demonstration?
- Is the AI integration flexible enough to prove good design?
- Are security and user-owned data handled correctly?

## Final Delegation Summary

| Area | Best Delegation Model |
| --- | --- |
| Product vision and scope | Human-led, AI-assisted |
| Architecture | Shared collaboration |
| Database modeling | AI-drafted, human-reviewed |
| AI provider integration | Shared collaboration |
| Prompt and report design | AI-executed, human-directed |
| Live research and citations | Human-governed, AI-assisted |
| Export system | AI-assisted, human QA |
| Authentication and report history | AI-assisted with human security review |
| MVC user interface | Shared collaboration |
| Testing and quality review | Shared, human final acceptance |
| Documentation | Human-led, AI-polished |

## Course Reflection

The main insight from this delegation analysis is that AI should be used as an accelerator and thinking partner, not as an owner of the project. The most important human role is discernment: deciding what matters, what is trustworthy, what is clear, and what demonstrates real engineering ability.

For this project, collaboration has the most impact in architecture, AI prompt design, report quality evaluation, and testing. These are the areas where AI can provide speed and breadth, while the human provides judgment and direction.
