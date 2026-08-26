<script lang="ts">
  // Dual-handle range slider. Works in whole units (here: seconds). The two
  // native range inputs are stacked over a shared track; only their thumbs are
  // interactive (pointer-events trick), so each handle drags independently.
  let {
    max,
    start = $bindable(0),
    end = $bindable(0),
    step = 1,
  }: {
    max: number;
    start?: number;
    end?: number;
    step?: number;
  } = $props();

  function onStartInput(e: Event) {
    const v = Number((e.target as HTMLInputElement).value);
    // Never let start pass end.
    start = Math.min(v, end);
  }

  function onEndInput(e: Event) {
    const v = Number((e.target as HTMLInputElement).value);
    // Never let end fall below start.
    end = Math.max(v, start);
  }

  const pct = (v: number) => (max > 0 ? Math.min(100, Math.max(0, (v / max) * 100)) : 0);
</script>

<div class="range-slider">
  <div class="track">
    <div class="track-fill" style="left:{pct(start)}%; right:{100 - pct(end)}%"></div>
  </div>
  <input
    type="range"
    class="thumb"
    min="0"
    {max}
    {step}
    value={start}
    oninput={onStartInput}
    aria-label="Trim start"
  />
  <input
    type="range"
    class="thumb"
    min="0"
    {max}
    {step}
    value={end}
    oninput={onEndInput}
    aria-label="Trim end"
  />
</div>

<style>
  .range-slider {
    position: relative;
    height: 28px;
    width: 100%;
  }

  .track {
    position: absolute;
    top: 50%;
    left: 0;
    right: 0;
    height: 4px;
    transform: translateY(-50%);
    background-color: var(--border-color);
    border-radius: 2px;
  }

  .track-fill {
    position: absolute;
    top: 0;
    bottom: 0;
    background-color: var(--primary-color);
    border-radius: 2px;
  }

  /* Both inputs occupy the full width, stacked over the track. The input body
     ignores pointer events so clicks fall through to whichever thumb is on top;
     the thumbs re-enable pointer events so they stay draggable. */
  input.thumb {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    margin: 0;
    background: none;
    -webkit-appearance: none;
    appearance: none;
    pointer-events: none;
  }

  input.thumb:focus {
    outline: none;
  }

  input.thumb::-webkit-slider-runnable-track {
    background: none;
    border: none;
    height: 100%;
  }

  input.thumb::-webkit-slider-thumb {
    -webkit-appearance: none;
    appearance: none;
    pointer-events: auto;
    width: 16px;
    height: 16px;
    border-radius: 50%;
    background-color: var(--primary-color);
    border: 2px solid #fff;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
    cursor: pointer;
  }

  /* Firefox fallbacks */
  input.thumb::-moz-range-track {
    background: none;
    border: none;
    height: 100%;
  }

  input.thumb::-moz-range-thumb {
    pointer-events: auto;
    width: 16px;
    height: 16px;
    border-radius: 50%;
    background-color: var(--primary-color);
    border: 2px solid #fff;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
    cursor: pointer;
  }
</style>
