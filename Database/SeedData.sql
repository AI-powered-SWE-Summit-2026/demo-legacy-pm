USE LegacyPMDb;
GO

INSERT INTO dbo.Resources (FullName, Email, Role, Department, HourlyRate, IsActive) VALUES
('Alicia Gomez', 'alicia.gomez@example.com', 'Project Manager', 'PMO', 125.00, 1),
('Bruno Costa', 'bruno.costa@example.com', 'Developer', 'LATAM', 95.00, 1),
('Charlotte Reed', 'charlotte.reed@example.com', 'QA Engineer', 'Operations', 88.00, 1),
('Diego Lima', 'diego.lima@example.com', 'Business Analyst', 'LATAM', 90.00, 1),
('Ethan Cole', 'ethan.cole@example.com', 'UX Designer', 'Product', 102.50, 0);
GO

INSERT INTO dbo.Projects (Name, Description, StartDate, EndDate, Status, OwnerId, Budget, ActualCost, CreatedAt, UpdatedAt) VALUES
('ERP Upgrade', 'Modernize back office workflows with a phased ERP rollout.', '2025-01-06', '2025-08-30', 'Active', 'alicia.gomez', 250000.00, 98420.00, '2025-01-01', '2025-05-15'),
('Customer Portal Refresh', 'Revamp the customer portal with responsive pages.', '2025-02-10', '2025-07-15', 'Planning', 'charlotte.reed', 90000.00, 15000.00, '2025-02-01', '2025-05-01'),
('Data Lake Migration', 'Move reporting workloads into the new analytics platform.', '2024-11-01', '2025-06-30', 'OnHold', 'diego.lima', 175000.00, 70250.00, '2024-10-20', '2025-04-20'),
('Support Knowledge Base', 'Build an internal knowledge base for support teams.', '2025-03-03', '2025-05-31', 'Completed', 'ethan.cole', 45000.00, 43600.00, '2025-03-01', '2025-05-20');
GO

INSERT INTO dbo.ProjectTasks (ProjectId, Title, Description, AssignedToId, DueDate, Priority, Status, EstimatedHours, ActualHours, CreatedAt) VALUES
(1, 'Assess current ERP modules', 'Inventory existing ERP customizations and integrations.', 4, '2025-01-20', 'High', 'Done', 24.00, 26.00, '2025-01-02'),
(1, 'Provision test environments', 'Set up legacy and target environments for validation.', 2, '2025-02-01', 'High', 'Done', 32.00, 34.50, '2025-01-05'),
(1, 'Create migration checklist', 'Document migration cutover checklist.', 1, '2025-02-18', 'Medium', 'Review', 18.00, 14.00, '2025-01-08'),
(1, 'Validate finance workflows', 'Run finance team UAT scenarios.', 3, '2025-06-02', 'Critical', 'InProgress', 40.00, 22.00, '2025-04-10'),
(2, 'Define portal sitemap', 'Outline navigation and responsive content blocks.', 5, '2025-03-15', 'Medium', 'Done', 12.00, 11.00, '2025-02-12'),
(2, 'Approve visual direction', 'Collect stakeholder sign-off on mockups.', 1, '2025-04-05', 'High', 'Review', 8.00, 6.00, '2025-03-01'),
(2, 'Implement landing page', 'Build Razor pages for public portal landing.', 2, '2025-04-25', 'High', 'InProgress', 28.00, 20.50, '2025-03-12'),
(2, 'Regression test responsive layout', 'Verify layout in legacy browsers.', 3, '2025-05-08', 'Medium', 'Todo', 16.00, 0.00, '2025-03-15'),
(3, 'Inventory SSIS jobs', 'List all SSIS packages and upstream data feeds.', 4, '2024-12-01', 'High', 'Done', 20.00, 21.50, '2024-11-02'),
(3, 'Build ADLS folder structure', 'Prepare Data Lake folder and retention strategy.', 2, '2025-01-10', 'Medium', 'Done', 22.00, 24.00, '2024-12-05'),
(3, 'Review governance checklist', 'Pause until data governance signoff is restored.', 1, '2025-06-15', 'Critical', 'OnHold', 14.00, 5.00, '2025-04-01'),
(4, 'Gather FAQ drafts', 'Collect support FAQ drafts from service desk.', 4, '2025-03-20', 'Low', 'Done', 10.00, 9.50, '2025-03-02'),
(4, 'Design article templates', 'Create simple knowledge base templates.', 5, '2025-03-28', 'Medium', 'Done', 14.00, 16.00, '2025-03-03'),
(4, 'Load initial articles', 'Publish first set of support articles.', 3, '2025-04-10', 'Medium', 'Done', 25.00, 23.75, '2025-03-05'),
(1, 'Prepare go-live communication', 'Draft rollout communication to business units.', 1, '2025-06-10', 'Medium', 'Todo', 6.00, 0.00, '2025-05-10');
GO

INSERT INTO dbo.Milestones (ProjectId, Name, Description, DueDate, IsCompleted, CompletedAt) VALUES
(1, 'Discovery Complete', 'Finalize discovery and planning documents.', '2025-02-15', 1, '2025-02-14'),
(1, 'UAT Signoff', 'Obtain business signoff after testing.', '2025-06-20', 0, NULL),
(2, 'Design Approved', 'Finalize refreshed design language.', '2025-04-10', 0, NULL),
(2, 'Beta Release', 'Roll out beta portal to pilot customers.', '2025-06-01', 0, NULL),
(3, 'Governance Review', 'Resume once governance committee approves controls.', '2025-06-18', 0, NULL),
(4, 'Knowledge Base Launch', 'Go-live for support article portal.', '2025-05-15', 1, '2025-05-14');
GO

INSERT INTO dbo.TimeEntries (ProjectTaskId, ResourceId, Date, Hours, Description, CreatedAt) VALUES
(1, 4, '2025-01-03', 4.00, 'Reviewed procurement workflow.', '2025-01-03'),
(1, 4, '2025-01-04', 5.50, 'Documented finance customizations.', '2025-01-04'),
(2, 2, '2025-01-10', 6.00, 'Built first test environment.', '2025-01-10'),
(2, 2, '2025-01-12', 5.00, 'Configured integration endpoints.', '2025-01-12'),
(3, 1, '2025-02-10', 3.50, 'Aligned checklist with PMO.', '2025-02-10'),
(4, 3, '2025-05-02', 4.00, 'Executed UAT cycle 1.', '2025-05-02'),
(4, 3, '2025-05-09', 6.50, 'Retested defects and evidence.', '2025-05-09'),
(5, 5, '2025-02-18', 4.00, 'Created new sitemap sketches.', '2025-02-18'),
(6, 1, '2025-03-20', 2.50, 'Prepared design review deck.', '2025-03-20'),
(7, 2, '2025-04-01', 5.25, 'Built portal hero section.', '2025-04-01'),
(7, 2, '2025-04-03', 4.75, 'Connected legacy CMS content.', '2025-04-03'),
(8, 3, '2025-04-20', 2.00, 'Tested layout in IE mode.', '2025-04-20'),
(9, 4, '2024-11-05', 3.25, 'Catalogued package dependencies.', '2024-11-05'),
(10, 2, '2024-12-12', 7.00, 'Configured storage account folders.', '2024-12-12'),
(11, 1, '2025-04-10', 1.50, 'Prepared governance decision log.', '2025-04-10'),
(12, 4, '2025-03-08', 3.00, 'Interviewed support specialists.', '2025-03-08'),
(13, 5, '2025-03-12', 6.00, 'Designed FAQ article template.', '2025-03-12'),
(14, 3, '2025-04-05', 5.50, 'Loaded first knowledge articles.', '2025-04-05'),
(15, 1, '2025-05-21', 2.00, 'Drafted rollout note for business.', '2025-05-21'),
(4, 3, '2025-05-14', 3.25, 'Prepared acceptance summary.', '2025-05-14');
GO

INSERT INTO dbo.ProjectMembers (ProjectId, ResourceId, Role, JoinedAt) VALUES
(1, 1, 'Executive Sponsor', '2025-01-01'),
(1, 2, 'Technical Lead', '2025-01-01'),
(2, 3, 'QA Lead', '2025-02-15');
GO

INSERT INTO dbo.NotificationLogs (RecipientEmail, Subject, Body, SentAt, Status, Culture) VALUES
('alicia.gomez@example.com', 'Kickoff confirmed', 'ERP kickoff meeting confirmed.', '2025-01-06', 'Sent', 'en-US'),
('bruno.costa@example.com', 'Prazo atualizado', 'O prazo da tarefa foi atualizado.', '2025-04-01', 'Sent', 'pt-BR'),
('charlotte.reed@example.com', 'Test cycle reminder', 'Please complete regression cycle.', '2025-05-03', 'Queued', 'en-US'),
('diego.lima@example.com', 'Governanca pendente', 'A aprovacao de governanca continua pendente.', '2025-04-11', 'Erro SMTP', 'pt-BR'),
('ethan.cole@example.com', 'Knowledge base launch', 'Launch confirmed for support portal.', '2025-05-14', 'Sent', 'en-US');
GO