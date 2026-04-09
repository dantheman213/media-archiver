import { writable, derived } from 'svelte/store';
import type { MediaJob, JobStatus, JobProgress, JobConfig, MediaMetadata } from '../types';

// ---------------------------------------------------------------------------
// Queue store – holds every MediaJob in the application
// ---------------------------------------------------------------------------

export const jobs = writable<MediaJob[]>([]);

// Derived convenience stores
export const activeJobs = derived(jobs, ($jobs) =>
  $jobs.filter((j) => !['completed', 'error'].includes(j.status))
);

export const completedJobs = derived(jobs, ($jobs) =>
  $jobs.filter((j) => j.status === 'completed')
);

export const errorJobs = derived(jobs, ($jobs) =>
  $jobs.filter((j) => j.status === 'error')
);

// ---------------------------------------------------------------------------
// Selection state – tracks which job is currently selected in the UI
// ---------------------------------------------------------------------------

export const selectedJobId = writable<string | null>(null);

// ---------------------------------------------------------------------------
// Mutation helpers
// ---------------------------------------------------------------------------

/** Create a default JobConfig */
function defaultConfig(): JobConfig {
  return {
    workflow: 'video_best',
    embedSubtitles: false,
    subtitleLanguages: [],
    embedMetadata: true,
    embedThumbnail: true,
  };
}

/** Create a default JobProgress */
function defaultProgress(): JobProgress {
  return {
    percentage: 0,
    downloadSpeed: '',
    eta: '',
    currentStep: '',
  };
}

/** Add a new job to the queue. Returns the generated id. */
export function addJob(url: string, fastAdd = false): string {
  const id = crypto.randomUUID();
  const job: MediaJob = {
    id,
    url,
    status: fastAdd ? 'queued' : 'inspecting',
    metadata: null,
    config: defaultConfig(),
    progress: defaultProgress(),
  };
  jobs.update((current) => [...current, job]);
  return id;
}

/** Update the status of a job by id */
export function updateJobStatus(id: string, status: JobStatus, errorMessage?: string): void {
  jobs.update((current) =>
    current.map((job) =>
      job.id === id ? { ...job, status, ...(errorMessage !== undefined ? { errorMessage } : {}) } : job
    )
  );
}

/** Update the progress of a job by id */
export function updateJobProgress(id: string, progress: Partial<JobProgress>): void {
  jobs.update((current) =>
    current.map((job) =>
      job.id === id ? { ...job, progress: { ...job.progress, ...progress } } : job
    )
  );
}

/** Update the metadata of a job by id */
export function updateJobMetadata(id: string, metadata: MediaMetadata): void {
  jobs.update((current) =>
    current.map((job) => (job.id === id ? { ...job, metadata } : job))
  );
}

/** Update the config of a job by id */
export function updateJobConfig(id: string, config: Partial<JobConfig>): void {
  jobs.update((current) =>
    current.map((job) =>
      job.id === id ? { ...job, config: { ...job.config, ...config } } : job
    )
  );
}

/** Remove a job from the queue by id */
export function removeJob(id: string): void {
  jobs.update((current) => current.filter((job) => job.id !== id));
  // Clear selection if the removed job was selected
  selectedJobId.update((sel) => (sel === id ? null : sel));
}

/** Clear all completed (and optionally errored) jobs */
export function clearCompleted(includeErrors = false): void {
  jobs.update((current) =>
    current.filter((job) => {
      if (job.status === 'completed') return false;
      if (includeErrors && job.status === 'error') return false;
      return true;
    })
  );
}
