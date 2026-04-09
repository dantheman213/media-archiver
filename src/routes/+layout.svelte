<script lang="ts">
  import '../app.css';
  import Onboarding from '../components/Onboarding.svelte';
  
  let { children } = $props();
  let onboardingComplete = $state(false);
</script>

{#if !onboardingComplete}
  <Onboarding onComplete={() => onboardingComplete = true} />
{/if}

{#if onboardingComplete}
  {@render children()}
{/if}