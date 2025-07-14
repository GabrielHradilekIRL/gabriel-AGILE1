document.addEventListener('DOMContentLoaded', () => {
  const questionLabels = [
    "Team size", "Team experience", "Remote or co-located", "Autonomy level", "Scrum roles?",
    "Project complexity", "Requirements clarity", "Frequent changes?", "Tech debt tolerance", "Predictable delivery",
    "Flat or hierarchical?", "Inter-team dependencies", "Governance heavy?", "Fixed scope/budget?", "Portfolio planning?",
    "Release frequency", "Flexible flow?", "Fixed cycles?", "Customer feedback?", "Push vs. pull workflow?",
    "Compliance needs?", "Audit trail?", "Risk tolerance", "SLAs?", "Security protocols?",
    "Change resistance?", "Kaizen culture?", "Leadership buy-in?", "Previous Agile use?", "Agile maturity level?"
  ];

  const stepsContainer = document.getElementById('steps');
  const totalSteps = 6;
  const perStep = 5;
  let stepIndex = 0;

  // Generate form steps and questions
  for (let s = 0; s < totalSteps; s++) {
    const stepDiv = document.createElement('div');
    stepDiv.className = `step ${s === 0 ? '' : 'hidden'} animate-fadeIn`;
    const h = document.createElement('h2');
    h.className = 'font-semibold mb-2';
    h.innerHTML = `Step ${s + 1}`;
    stepDiv.appendChild(h);

    for (let i = 0; i < perStep; i++) {
      const idx = s * perStep + i;
      const wrapper = document.createElement('label');
      wrapper.className = "block mb-3";
      wrapper.innerHTML = `
        <div class="mb-1">${idx + 1}. ${questionLabels[idx]}</div>
        <select name="q${idx + 1}" required class="mt-1 block w-full border rounded p-2">
          <option value="">Select...</option>
          <option value="1">Low</option>
          <option value="2">Medium</option>
          <option value="3">High</option>
        </select>
      `;
      stepDiv.appendChild(wrapper);
    }

    stepsContainer.appendChild(stepDiv);
  }

  const steps = document.querySelectorAll('.step');
  const prevBtn = document.getElementById('prevBtn');
  const nextBtn = document.getElementById('nextBtn');
  const progress = document.getElementById('progress');

  function showStep(n) {
    steps.forEach((s, i) => s.classList.toggle('hidden', i !== n));
    prevBtn.disabled = n === 0;
    nextBtn.textContent = n === steps.length - 1 ? "Submit" : "Next";
    progress.style.width = `${(n / (steps.length - 1)) * 100}%`;
  }

  prevBtn.onclick = () => {
    if (stepIndex > 0) stepIndex--;
    showStep(stepIndex);
  };

  nextBtn.onclick = () => {
    const selects = steps[stepIndex].querySelectorAll('select');
    for (let s of selects) {
      if (!s.value) {
        s.focus();
        return;
      }
    }
    if (stepIndex < steps.length - 1) {
      stepIndex++;
      showStep(stepIndex);
    } else {
      calculateRecommendation();
    }
  };

  function calculateRecommendation() {
    const data = new FormData(document.getElementById('wizard'));
    const answers = Array.from(data.values()).map(v => parseInt(v));
    const score = { Scrum: 0, Kanban: 0, SAFe: 0, Scrumban: 0 };

    answers.forEach((val, idx) => {
      if (idx % 4 === 0) score.Scrum += val;
      else if (idx % 4 === 1) score.Kanban += val;
      else if (idx % 4 === 2) score.SAFe += val;
      else score.Scrumban += val;
    });

    const [best] = Object.entries(score).sort((a, b) => b[1] - a[1]);

    const frameworks = {
      Scrum: {
        pros: "Great for iterative work and clear roles.",
        cons: "Requires commitment and ceremonies.",
        steps: "Start with sprints, standups, retros, and backlog grooming."
      },
      Kanban: {
        pros: "Simple to adopt, great for continuous flow.",
        cons: "Can become chaotic without WIP limits.",
        steps: "Visualize the flow, limit WIP, manage queues."
      },
      SAFe: {
        pros: "Ideal for large-scale Agile across enterprises.",
        cons: "More overhead, more governance.",
        steps: "Create ARTs, define roles, plan PIs."
      },
      Scrumban: {
        pros: "Blends Scrum structure with Kanban flexibility.",
        cons: "Requires self-discipline and clarity.",
        steps: "Use a Kanban board with Scrum rituals."
      }
    };

    document.getElementById('chosen').textContent = best[0];
    document.getElementById('pros').textContent = frameworks[best[0]].pros;
    document.getElementById('cons').textContent = frameworks[best[0]].cons;
    document.getElementById('steps').textContent = frameworks[best[0]].steps;

    document.getElementById('wizard').classList.add('hidden');
    document.getElementById('summary').classList.remove('hidden');
    progress.style.width = "100%";
  }

  showStep(stepIndex);
});
