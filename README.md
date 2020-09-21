# PAQ-U170

This game is a side project that has been around for about almost two years*, but the real development time was about only one year. It started as an experiment for learning how to develop a small serious game with my friend @sariusdev as a 3D artist; our mate @_matrioshka joined us recently to help us with narrative design and audiovisual production.

Our main focus was to finish a game with a small scope, out of a "game jam game".

*As a result of that, I'm not 100% happy with the code base, since many things didn't need to be modified or were done at the beginning.

## Play it!
https://xiphereal.itch.io/paq-u170

## What did I learn
- How to work with an agile philosophy in a multidisciplinary team:
  - Being the Scrum Master with a non-technical team, teaching the basics of agile soft. dev.
- Working alongside an artist workflow, knowing its needs and many other 3D art related stuff:
  - Basic understanding of the 3D artist role, what is a good practice, best approaches that lead to more efficient work and less workload to him.
  - Pivot/axis conventions exporting from 3D modeling tools (Blender) to Unity, and many of the little things that need to be taken into account.
- The basics of working with Unity Universal Render Pipeline.
- Deep dive into the 3D lightning world (more specifically in URP, but also applicable to any other RP or engine):
  - Evaluate the pros/cons of baking the light or using a mixed approach (like Unity precomputed Global Illumination) 
  - Concept of texels in relation to lightmap resolution, offset between isles, etc
  - How to achieve a good balance between quality/accuracy and baking time.
  - How to create compeling scenes and think creatively about asset composition
  - Working with lighting limitations (URP only allows 1 directional lights and limits the realtime lights/shadows texture resolution)
- How to manage the needs of a longer and bigger project in relation to a game jam game, mainly code practices and architechtural needs.
 
