# Frontend Migration Plan (Blazor -> Angular)

## Goal

Replace `BookBooks.Web` with `bookbooks-web` (Angular) without blocking backend evolution.

## Principles

- Incremental migration by feature
- Backend contract-first
- Keep legacy Blazor available until parity
- No breaking API changes only for UI migration

## Phase 0 - Bootstrap

1. Create Angular app in `bookbooks-web/`
2. Configure environments (`apiBaseUrl`)
3. Add core HTTP layer:
   - auth interceptor
   - API error handler
   - route guards
4. Wire base layout + navigation skeleton

## Phase 1 - Auth

1. Register page
2. Login page
3. Session/token lifecycle
4. Route protection

Done when:

- user can register, login, logout in Angular
- protected routes reject anonymous users

## Phase 2 - Books + Reviews

1. Books search/list
2. Book detail
3. Create review
4. Update/delete review
5. Toggle like

Done when:

- all current books/reviews flows from Blazor work in Angular

## Phase 3 - Reading Status + Lists

1. Reading status get/upsert
2. Lists CRUD
3. Add/remove book in list

Done when:

- list and reading status endpoints are fully consumed in Angular

## Phase 4 - Follows + Feed

1. Follow/unfollow management
2. My following screen
3. Feed timeline implementation

Done when:

- follows and feed are available end-to-end in Angular

## Phase 5 - Decommission

1. Validate feature parity checklist
2. Remove Blazor navigation/routes
3. Remove `BookBooks.Web` project after final approval
4. Update docs and CI

## Tracking Checklist

- [x] Angular app scaffolded
- [x] Auth migrated
- [x] Books migrated
- [x] Reviews migrated
- [x] Reading status migrated
- [x] Lists migrated
- [x] Follows migrated
- [x] Feed implemented
- [ ] Blazor retired
