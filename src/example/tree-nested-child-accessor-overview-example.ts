import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MatTreeModule } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

/**
 * Food data with nested structure.
 * Each node has a name and an optional list of children.
 */
interface FoodNode {
  name: string;
  children?: FoodNode[];
}

/**
 * @title Tree with nested nodes (childrenAccessor)
 */
@Component({
  selector: 'tree-nested-child-accessor-overview-example',
  templateUrl: 'tree-nested-child-accessor-overview-example.html',
  styleUrl: 'tree-nested-child-accessor-overview-example.css',
  imports: [MatTreeModule, MatButtonModule, MatIconModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TreeNestedChildAccessorOverviewExample {
  childrenAccessor = (node: FoodNode) => node.children ?? [];

  dataSource = LINUX_FS_DATA;

  hasChild = (_: number, node: FoodNode) =>
    !!node.children && node.children.length > 0;
}

const LINUX_FS_DATA: FoodNode[] = [
  {
    name: '/', // Level 1 (Root)
    children: [
      {
        name: 'home', // Level 2
        children: [
          {
            name: 'user', // Level 3
            children: [
              {
                name: 'projects', // Level 4
                children: [
                  { name: 'portfolio' }, // Level 5 (Leaf)
                  { name: 'scripts' }, // Level 5 (Leaf)
                  { name: 'README.md' }, // Level 5 (Leaf)
                ],
              },
              {
                name: 'documents', // Level 4
                children: [
                  { name: 'taxes.pdf' }, // Level 5
                  { name: 'resume.docx' }, // Level 5
                ],
              },
            ],
          },
        ],
      },
      {
        name: 'usr', // Level 2
        children: [
          {
            name: 'local', // Level 3
            children: [
              {
                name: 'bin', // Level 4
                children: [
                  { name: 'docker' }, // Level 5
                  { name: 'node' }, // Level 5
                  { name: 'git' }, // Level 5
                ],
              },
            ],
          },
        ],
      },
      {
        name: 'var', // Level 2
        children: [
          {
            name: 'www', // Level 3
            children: [
              {
                name: 'html', // Level 4
                children: [
                  { name: 'index.html' }, // Level 5
                  { name: 'style.css' }, // Level 5
                  { name: 'app.js' }, // Level 5
                ],
              },
            ],
          },
        ],
      },
    ],
  },
];

/**  Copyright 2026 Google LLC. All Rights Reserved.
    Use of this source code is governed by an MIT-style license that
    can be found in the LICENSE file at https://angular.io/license */
